﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Octoller.BotBox.Web.Models;

namespace Octoller.BotBox.Web.Data.Configurations {

    public class CommunityModelConfiguration : IEntityTypeConfiguration<Community> {

        public void Configure(EntityTypeBuilder<Community> builder) {

            builder.ToTable("Communities");
            builder.HasKey(c => c.Id);
            builder.HasAlternateKey(c => c.VkId);

            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(500);
            builder.Property(c => c.Photo).HasColumnType("varbinary(max)");
            builder.Property(c => c.VkId).IsRequired().HasMaxLength(100);
            builder.Property(c => c.AccessToken).HasMaxLength(250);
            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.CreateBy).IsRequired().HasMaxLength(100);
            builder.Property(c => c.UpdateAt);
            builder.Property(c => c.UpdateBy).HasMaxLength(100);
            builder.Property(c => c.Connected);

            builder.HasIndex(c => c.Name);

            builder.HasOne(c => c.User).WithOne(u => u.Community);

        }
    }
}