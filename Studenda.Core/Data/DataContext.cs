using Microsoft.EntityFrameworkCore;
using Studenda.Core.Data.Configuration;
using Studenda.Core.Model;
using Studenda.Core.Model.Account;
using Studenda.Core.Model.Common;
using Studenda.Core.Model.Link;

namespace Studenda.Core.Data;

/// <summary>
/// Сессия работы с базой данных.
/// 
/// TODO: Создать класс управления контекстами и их конфигурациями.
///
/// Памятка для работы с кешем:
/// - context.Add() для запроса INSERT.
///   Объекты вставляются со статусом Added.
///   При коммите изменений произойдет попытка вставки.
/// - context.Update() для UPDATE.
///   Объекты вставляются со статусом Modified.
///   При коммите изменений произойдет попытка обновления.
/// - context.Attach() для вставки в кеш.
///   Объекты вставляются со статусом Unchanged.
///   При коммите изменений ничего не произойдет.
/// </summary>
public abstract class DataContext : DbContext
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="configuration">Конфигурация базы данных.</param>
    protected DataContext(DatabaseConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// Конфигурация базы данных.
    /// </summary>
    private DatabaseConfiguration Configuration { get; }

    /// <summary>
    /// Набор объектов <see cref="User"/>.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Набор объектов <see cref="Role"/>.
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>
    /// Набор объектов <see cref="Permission"/>.
    /// </summary>
    public DbSet<Permission> Permissions => Set<Permission>();

    /// <summary>
    /// Набор объектов <see cref="Department"/>.
    /// </summary>
    public DbSet<Department> Departments => Set<Department>();

    /// <summary>
    /// Набор объектов <see cref="Course"/>.
    /// </summary>
    public DbSet<Course> Courses => Set<Course>();

    /// <summary>
    /// Набор объектов <see cref="Group"/>.
    /// </summary>
    public DbSet<Group> Groups => Set<Group>();

    /// <summary>
    /// Набор объектов <see cref="WeekType"/>.
    /// </summary>
    public DbSet<WeekType> WeekTypes => Set<WeekType>();

    /// <summary>
    /// Набор объектов <see cref="UserGroupLink"/>.
    /// </summary>
    public DbSet<UserGroupLink> UserGroupLinks => Set<UserGroupLink>();

    /// <summary>
    /// Набор объектов <see cref="RolePermissionLink"/>.
    /// </summary>
    public DbSet<RolePermissionLink> RolePermissionLinks => Set<RolePermissionLink>();

    /// <summary>
    /// Обработать инициализацию модели.
    /// Используется для дополнительной настройки модели.
    /// </summary>
    /// <param name="modelBuilder">Набор интерфейсов настройки модели.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Использование Fluent API.
        modelBuilder.ApplyConfiguration(new User.Configuration(Configuration));
        modelBuilder.ApplyConfiguration(new Role.Configuration(Configuration));
        modelBuilder.ApplyConfiguration(new Permission.Configuration(Configuration));
        modelBuilder.ApplyConfiguration(new Department.Configuration(Configuration));
        modelBuilder.ApplyConfiguration(new Course.Configuration(Configuration));
        modelBuilder.ApplyConfiguration(new Group.Configuration(Configuration));
        modelBuilder.ApplyConfiguration(new WeekType.Configuration(Configuration));
        modelBuilder.ApplyConfiguration(new UserGroupLink.Configuration());
        modelBuilder.ApplyConfiguration(new RolePermissionLink.Configuration());

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Сохранить все изменения сессии в базу данных.
    /// Используется для обновления метаданных модели.
    /// </summary>
    /// <returns>Количество затронутых записей.</returns>
    public override int SaveChanges()
    {
        UpdateTrackedEntityMetadata();

        return base.SaveChanges();
    }

    /// <summary>
    /// Асинхронно сохранить все изменения сессии в базу данных.
    /// Используется для обновления метаданных модели.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Флаг принятия всех изменений при успехе операции.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Таск, представляющий операцию асинхронного сохранения с количеством затронутых записей.</returns>
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        UpdateTrackedEntityMetadata();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// Обновить метаданные всех добавленных
    /// и модифицироанных моделей в кеше сессии.
    /// Такой подход накладывает дополнительные ограничения
    /// при работе с сессиями. Необходимо учитывать, что
    /// для обновления записей нужно сперва загрузить эти
    /// записи в кеш сессии, чтобы трекер корректно
    /// зафиксировал изменения.
    /// TODO: Возможно, это не лучшее решение. Необходимы тесты.
    /// </summary>
    private void UpdateTrackedEntityMetadata()
    {
        var entries = ChangeTracker.Entries().Where(entry => (entry.Entity is Entity)
            && (entry.State == EntityState.Added || entry.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.Entity is not Entity entity)
            {
                continue;
            }

            // Добавлен новый объект.
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.Now;
            }

            // Обновлен существующий объект.
            if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.Now;
            }
        }
    }
}