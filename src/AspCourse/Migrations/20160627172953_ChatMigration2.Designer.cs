﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AspCourse.Data;

namespace AspCourse.Migrations
{
    [DbContext(typeof(ChatDbContext))]
    [Migration("20160627172953_ChatMigration2")]
    partial class ChatMigration2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rc2-20901")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AspCourse.Models.ChatModels.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("AspCourse.Models.ChatModels.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("TopicId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("AspCourse.Models.ChatModels.Topic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsClosed");

                    b.Property<bool>("IsSticky");

                    b.Property<int>("OpPostId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Topics");
                });
        }
    }
}
