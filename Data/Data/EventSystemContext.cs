using System;
using System.Collections.Generic;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Data;

public partial class EventSystemContext : DbContext
{
    public EventSystemContext()
    {
    }

    public EventSystemContext(DbContextOptions<EventSystemContext> options) : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<SessionRegistration> SessionRegistrations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=EventSystem;User Id=sa;Password=MysqlPassword123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Event__3214EC070B473F7A");

            entity.ToTable("Event");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.EventType).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Session__3214EC07A3F27582");

            entity.ToTable("Session");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.RoomName).HasMaxLength(50);
            entity.Property(e => e.SpeakerName).HasMaxLength(100);
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Event).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK__Session__EventId__3C69FB99");
        });

        modelBuilder.Entity<SessionRegistration>(entity =>
        {
            entity.HasKey(e => new { e.SessionId, e.UserId }).HasName("PK__SessionR__188C1E54ED228B91");

            entity.ToTable("SessionRegistration");

            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Session).WithMany(p => p.SessionRegistrations)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("FK__SessionRe__Sessi__3F466844");

            entity.HasOne(d => d.User).WithMany(p => p.SessionRegistrations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__SessionRe__UserI__403A8C7D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC072529D85E");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D1053450727A3D").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
