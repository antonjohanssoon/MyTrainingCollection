using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Databases
{
    public class Database : DbContext
    {
        public Database(DbContextOptions<Database> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }
        public DbSet<Exercise> Exercises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // WorkoutSession -> User (1..*), kräver att WorkoutSession har public Guid UserId { get; set; }
            modelBuilder.Entity<WorkoutSession>()
                .HasOne(workoutSession => workoutSession.User)
                .WithMany(user => user.WorkoutSessions)
                .HasForeignKey(workoutSession => workoutSession.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // WorkoutSession -> Exercises (1..*)
            // Exercise har ingen back-navigation, så .WithOne() och shadow FK "WorkoutSessionId"
            modelBuilder.Entity<WorkoutSession>()
                .HasMany(workoutSession => workoutSession.Exercises)
                .WithOne()
                .HasForeignKey("WorkoutSessionId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Arv: TPH (table-per-hierarchy) med tydlig discriminator
            modelBuilder.Entity<Exercise>()
                .ToTable("Exercises")
                .HasDiscriminator<string>("ExerciseType")
                .HasValue<StrengthExercise>("Strength")
                .HasValue<CardioExercise>("Cardio");

            // Små constraints (valfritt men bra):
            modelBuilder.Entity<Exercise>()
                .Property(exercise => exercise.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<WorkoutSession>()
                .Property(workoutSession => workoutSession.Notes)
                .HasMaxLength(1000);

            modelBuilder.Entity<User>()
                .HasIndex(user => user.Username)
                .IsUnique();
        }
    }
}
