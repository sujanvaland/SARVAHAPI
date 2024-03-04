using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using static SpiritualNetwork.API.Model.TimelineModel;

namespace SpiritualNetwork.API.AppContext
{
    public class AppDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;
        public DbSet<User> Users { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequest { get; set; }
        public DbSet<EmailTemplate> EmailTemplate { get; set; }
        public DbSet<GlobalSetting> GlobalSetting { get; set; }
        public DbSet<PreRegisteredUser> PreRegisteredUsers { get; set; }
        public DbSet<Entities.File> Files { get; set; }
        public DbSet<Model.TimelineModel.UserChatResponse> UserChatResponse { get; set; }
        public DbSet<OnlineUsers> OnlineUsers { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<UserNotification> UserNotification { get; set; }
        public DbSet<UserSubcription> UserSubcription { get; set; }
        public DbSet<MessageGroupDetails> MessageGroupDetail { get; set; }
        public DbSet<GroupMember> GroupMember { get; set; }
        public DbSet<SnoozeUser> SnoozeUser { get; set; }
        public DbSet<Poll> Poll { get; set; }
        public DbSet<PollVote> PollVote { get; set; }
        public DbSet<JobExperience> JobExperience { get; set; }
        public DbSet<JobPost> JobPost { get; set; }
        public DbSet<Application> Application { get; set; }
        public DbSet<Reaction> Reaction { get; set; }


        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<UserChatResponse>()
               .HasNoKey()
               .ToTable("UserChatResponse", t => t.ExcludeFromMigrations());

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("Default"));
        }

    }
}
