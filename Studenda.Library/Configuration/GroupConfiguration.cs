﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studenda.Library.Model;

namespace Studenda.Library.Configuration;

/// <summary>
/// Конфигурация модели <see cref="Group"/>.
/// </summary>
public class GroupConfiguration : EntityConfiguration<Group>
{
    /// <summary>
    /// Задать конфигурацию для модели.
    /// </summary>
    /// <param name="builder">Набор интерфейсов настройки модели.</param>
    public override void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.Property(group => group.Name)
            .HasMaxLength(Group.NameLengthMax);

        builder.HasOne(group => group.Course)
            .WithMany(course => course.Groups)
            .HasForeignKey(group => group.CourseId);

        base.Configure(builder);
    }
}