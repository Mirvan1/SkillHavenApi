using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using SkillHaven.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Numerics;

namespace SkillHaven.Infrastructure.Data
{
    public class shDbContext : DbContext
    {
        private readonly string _connectionString;

        //public shDbContext(string connectionString)
        //{
        //    _connectionString = connectionString;
        //}
        //public shDbContext()
        //{
        //}

        public shDbContext(DbContextOptions<shDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<Consultant> Consultants { get; set; }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogComments> BlogComments { get; set; }

        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<ChatUserConnection> ChatUserConnections { get; set; }
        public DbSet<Message> Messages { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUser(modelBuilder.Entity<User>());
            ConfigureConsultant(modelBuilder.Entity<Consultant>());
            ConfigureSupervisor(modelBuilder.Entity<Supervisor>());
            ConfigureBlog(modelBuilder.Entity<Blog>());
            ConfigureBlogComments(modelBuilder.Entity<BlogComments>());
            ConfigureChatUser(modelBuilder.Entity<ChatUser>());
            ConfigureChatUserConnection(modelBuilder.Entity<ChatUserConnection>());
            ConfigureMessages(modelBuilder.Entity<Message>());



        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
                var connStr = configuration.GetConnectionString("DefaultConnection");
                //optionsBuilder.UseLazyLoadingProxies(true);

                optionsBuilder.UseSqlServer(connStr).UseLazyLoadingProxies(true); // Use connStr instead of _connectionString

            }
        }


        private void ConfigureUser(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.UserId);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
            builder.Property(u => u.Password).IsRequired();
            builder.Property(u => u.Role).IsRequired();
            builder.Property(u => u.FirstName).HasMaxLength(50);
            builder.Property(u => u.LastName).HasMaxLength(50);
            builder.Property(u => u.ProfilePicture).HasMaxLength(255);

            // Configure User-Supervisor relationship
            builder.HasOne(u => u.Supervisor)
                .WithOne(s => s.User)
                .HasForeignKey<Supervisor>(s => s.UserId);

            // Configure User-Consultant relationship
            builder.HasOne(u => u.Consultant)
                .WithOne(c => c.User)
                .HasForeignKey<Consultant>(c => c.UserId);

            builder.HasMany(u => u.Blogs)
                 .WithOne(c => c.User)
                 .HasForeignKey(c => c.UserId);

            builder.HasMany(b => b.BlogComments)
                 .WithOne(bc => bc.User)
                 .HasForeignKey(bc => bc.BlogCommentsId);


        }

        private void ConfigureConsultant(EntityTypeBuilder<Consultant> builder)
        {
            builder.ToTable("Consultant"); // Tablo adını belirle
            builder.HasKey(c => c.ConsultantId);
            builder.Property(c => c.Experience).IsRequired();
            builder.Property(c => c.Description).HasMaxLength(255);
            builder.Property(c => c.Rating).IsRequired();

            builder.HasOne(c => c.User)
         .WithOne(u => u.Consultant)
         .HasForeignKey<Consultant>(c => c.UserId);

        }


        private void ConfigureSupervisor(EntityTypeBuilder<Supervisor> builder)
        {
            builder.ToTable("Supervisors"); // Tablo adını belirle
            builder.HasKey(s => s.SupervisorId);
            builder.Property(s => s.Expertise).HasMaxLength(100);
            builder.Property(s => s.Description).HasMaxLength(255);
            builder.Property(c => c.Rating).IsRequired();

            // User ile ilişkiyi belirle
            builder.HasOne(s => s.User)
                .WithOne(u => u.Supervisor)
                .HasForeignKey<Supervisor>(s => s.UserId);
        }

        private void ConfigureBlog(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable("Blogs");
            builder.HasKey(b => b.BlogId);
            builder.Property(b => b.Title).HasMaxLength(255).IsRequired();
            builder.Property(b => b.Content).IsRequired();
            builder.Property(b => b.UserId).IsRequired();
            builder.Property(b => b.PublishDate).IsRequired();
            builder.Property(b => b.UpdateDate).IsRequired();
            builder.Property(b => b.IsPublished).IsRequired();
            builder.Property(b => b.Vote);

            builder.HasOne(b => b.User)
                .WithMany(u => u.Blogs)
                .HasForeignKey(b => b.UserId);

            builder.HasMany(b => b.BlogComments)
        .WithOne(bc => bc.Blog)
        .HasForeignKey(bc => bc.BlogId);
        }

        private void ConfigureBlogComments(EntityTypeBuilder<BlogComments> builder)
        {
            builder.ToTable("BlogComments");
            builder.HasKey(bc => bc.BlogCommentsId);
            builder.Property(bc => bc.CommentTitle).HasMaxLength(255).IsRequired();
            builder.Property(bc => bc.CommentContent).IsRequired();
            builder.Property(bc => bc.PublishDate).IsRequired();
            builder.Property(bc => bc.isPublished).IsRequired();

            builder.HasOne(bc => bc.Blog)
                .WithMany(b => b.BlogComments)
                .HasForeignKey(bc => bc.BlogId);

            builder.HasOne(bc => bc.User)
                .WithMany(b => b.BlogComments)
                .HasForeignKey(bc => bc.UserId);
        }


        private void ConfigureChatUser(EntityTypeBuilder<ChatUser> builder)
        {
            builder.ToTable("ChatUser");
            builder.HasKey(cu => cu.Id);
            builder.Property(cu => cu.UserId).IsRequired();
            builder.Property(cu => cu.LastSeen).IsRequired();
            builder.Property(cu => cu.Status).HasMaxLength(50); // Adjust size as needed
            builder.Property(cu => cu.ProfilePicture).HasMaxLength(255); // Assuming a URL or file path; adjust size as needed

            // Relationships
            builder.HasMany(cu => cu.SentMessages)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            builder.HasMany(cu => cu.ReceivedMessages)
                .WithOne(m => m.Receiver)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            builder.HasMany(cu => cu.UserConnections)
                .WithOne(uc => uc.ChatUser)
                .HasForeignKey(uc => uc.ChatUserId);
        }

        private void ConfigureMessages(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Message");
            builder.HasKey(m => m.MessageId);
            builder.Property(m => m.Content).IsRequired();
            builder.Property(m => m.Timestamp).IsRequired();
            builder.Property(m => m.MessageType).HasMaxLength(50); // Adjust size as needed
            builder.Property(m => m.SeenStatus).HasMaxLength(50); // Adjust size as needed

            builder.HasOne(m => m.Sender)
                .WithMany(cu => cu.SentMessages)
                .HasForeignKey(m => m.SenderId);

            builder.HasOne(m => m.Receiver)
                .WithMany(cu => cu.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId);
        }

        private void ConfigureChatUserConnection(EntityTypeBuilder<ChatUserConnection> builder)
        {
            builder.ToTable("UserConnection");
            builder.HasKey(uc => uc.Id);
            builder.Property(uc => uc.ConnectionId).HasMaxLength(255).IsRequired(); // Adjust size based on expected length of connection IDs

            builder.HasOne(uc => uc.ChatUser)
                .WithMany(cu => cu.UserConnections)
                .HasForeignKey(uc => uc.ChatUserId);
        }

    }
}
