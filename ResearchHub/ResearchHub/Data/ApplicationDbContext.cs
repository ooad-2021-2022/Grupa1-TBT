using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResearchHub.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResearchHub.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<ResearchPaper> ResearchPaper { get; set; }
        public DbSet<PaperAuthor> PaperAuthor { get; set; }
        public DbSet<PublishedPapers> PublishedPapers { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<Quiz> Quiz { get; set; }
        public DbSet<Ratings> Ratings { get; set; }
        public DbSet<Downloads> Downloads { get; set; }
        public DbSet<Collaborations> Collaborations { get; set; }
        public DbSet<UserSkills> UserSkills { get; set; }
        public DbSet<ResearchTopicsPaper> ResearchTopicsPaper { get; set; }
        public DbSet<PaperType> PaperType { get; set; }
        public DbSet<Requests> Requests { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().ToTable("User");
            builder.Entity<ResearchPaper>().ToTable("ResearchPaper");
            builder.Entity<PaperAuthor>().ToTable("PaperAuthor");
            builder.Entity<PublishedPapers>().ToTable("PublishedPapers");
            builder.Entity<Questions>().ToTable("Questions");
            builder.Entity<Quiz>().ToTable("Quiz");
            builder.Entity<Ratings>().ToTable("Ratings");
            builder.Entity<Downloads>().ToTable("Downloads");
            builder.Entity<Collaborations>().ToTable("Collaborations");
            builder.Entity<UserSkills>().ToTable("UserSkills");
            builder.Entity<ResearchTopicsPaper>().ToTable("ResearchTopicsPaper");
            builder.Entity<PaperType>().ToTable("PaperType");
            builder.Entity<Requests>().ToTable("Requests");
            /*builder.Entity<User>().HasRequired(c => c.Stage)
                .WithMany()
                .WillCascadeOnDelete(false);

            builder.Entity<Collaborations>()
                .HasRequired(s => s.Stage)
                .WithMany()
                .WillCascadeOnDelete(false);*/
            base.OnModelCreating(builder);
        }
    }
}
