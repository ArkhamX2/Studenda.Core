using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studenda.Core.Data.Configuration;

namespace Studenda.Core.Model.Common;

/// <summary>
/// Факультет.
/// </summary>
public class Department : Entity
{
    /// <summary>
    /// Конфигурация модели <see cref="Department"/>.
    /// </summary>
    internal class Configuration : Configuration<Department>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="configuration">Конфигурация базы данных.</param>
        public Configuration(ContextConfiguration configuration) : base(configuration) { }

        /// <summary>
        /// Задать конфигурацию для модели.
        /// </summary>
        /// <param name="builder">Набор интерфейсов настройки модели.</param>
        public override void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.Property(department => department.Name)
                .HasMaxLength(NameLengthMax)
                .IsRequired(IsNameRequired);

            builder.HasMany(department => department.Courses)
                .WithOne(course => course.Department)
                .HasForeignKey(course => course.DepartmentId);

            base.Configure(builder);
        }
    }
    
    /*                   __ _                       _   _
     *   ___ ___  _ __  / _(_) __ _ _   _ _ __ __ _| |_(_) ___  _ __
     *  / __/ _ \| '_ \| |_| |/ _` | | | | '__/ _` | __| |/ _ \| '_ \
     * | (_| (_) | | | |  _| | (_| | |_| | | | (_| | |_| | (_) | | | |
     *  \___\___/|_| |_|_| |_|\__, |\__,_|_|  \__,_|\__|_|\___/|_| |_|
     *                        |___/
     * Константы, задающие базовые конфигурации полей
     * и ограничения модели.
     */
    #region Configuration

    /// <summary>
    /// Максимальная длина поля <see cref="Name"/>.
    /// </summary>
    public const int NameLengthMax = 128;

    /// <summary>
    /// Статус необходимости наличия значения в поле <see cref="Name"/>.
    /// </summary>
    public const bool IsNameRequired = true;

    #endregion

    /*             _   _ _
     *   ___ _ __ | |_(_) |_ _   _
     *  / _ \ '_ \| __| | __| | | |
     * |  __/ | | | |_| | |_| |_| |
     *  \___|_| |_|\__|_|\__|\__, |
     *                       |___/
     * Поля данных, соответствующие таковым в таблице
     * модели в базе данных.
     */
    #region Entity

    /// <summary>
    /// Название.
    /// </summary>
    public string Name { get; set; } = null!;

    #endregion

    /// <summary>
    /// Связанные объекты <see cref="Course"/>.
    /// </summary>
    public List<Course> Courses { get; set; } = null!;
}
