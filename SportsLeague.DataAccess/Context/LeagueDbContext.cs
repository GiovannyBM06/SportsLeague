using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SportsLeague.Domain.Entities;

namespace SportsLeague.DataAccess.Context
{
    public class LeagueDbContext : DbContext
    {
        public LeagueDbContext(DbContextOptions<LeagueDbContext> options)
            : base(options)
        {
        }

        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Referee> Referees => Set<Referee>();
        public DbSet<Tournament> Tournaments => Set<Tournament>();
        public DbSet<TournamentTeam> TournamentTeams => Set<TournamentTeam>();
        public DbSet<Sponsor> Sponsors => Set<Sponsor>(); 
        public DbSet<TournamentSponsor> TournamentSponsors => Set<TournamentSponsor>();
        public DbSet<Match> Matches => Set <Match>();
        public DbSet<MatchResult> MatchResults => Set<MatchResult>();
        public DbSet<Goal> Goals => Set<Goal>();
        public DbSet<Card> Cards => Set<Card>();
        public DbSet<MatchLineup> MatchLineups => Set<MatchLineup>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // ── Team Configuration ──
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(t => t.City)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(t => t.Stadium)
                      .HasMaxLength(150);
                entity.Property(t => t.LogoUrl)
                      .HasMaxLength(500);
                entity.Property(t => t.CreatedAt)
                      .IsRequired();
                entity.Property(t => t.UpdatedAt)
                      .IsRequired(false);
                entity.HasIndex(t => t.Name)
                      .IsUnique();
            });
            // -Player Configuration-
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(80);
                entity.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(80);
                entity.Property(p => p.BirthDate)
                .IsRequired();
                entity.Property(p => p.Number)
                .IsRequired();
                entity.Property(p => p.Position)
                .IsRequired();
                entity.Property(p => p.CreatedAt)
                .IsRequired();
                entity.Property(p => p.UpdatedAt)
                .IsRequired(false);

                //Relación 1:N con Team
                entity.HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

                //Índice único compuesto: Nº Camiseta único por equipo
                entity.HasIndex(p => new { p.TeamId, p.Number })
                .IsUnique();
            });

            // -Referee conf.--
            modelBuilder.Entity<Referee>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.FirstName)
                      .IsRequired()
                      .HasMaxLength(80);
                entity.Property(r => r.LastName)
                      .IsRequired()
                      .HasMaxLength(80);
                entity.Property(r => r.Nationality)
                      .IsRequired()
                      .HasMaxLength(80);
                entity.Property(r => r.CreatedAt)
                      .IsRequired();
                entity.Property(r => r.UpdatedAt)
                      .IsRequired(false);
            });

            // Tournament Conf.
            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name)
                      .IsRequired()
                      .HasMaxLength(150);
                entity.Property(t => t.Season)
                      .IsRequired()
                      .HasMaxLength(20);
                entity.Property(t => t.StartDate)
                      .IsRequired();
                entity.Property(t => t.EndDate)
                      .IsRequired();
                entity.Property(t => t.Status)
                      .IsRequired();
                entity.Property(t => t.CreatedAt)
                      .IsRequired();
                entity.Property(t => t.UpdatedAt)
                      .IsRequired(false);
            });
            // TournamentTeam Conf.
            modelBuilder.Entity<TournamentTeam>(entity =>
            {
                entity.HasKey(tt => tt.Id);
                entity.Property(tt => tt.RegisteredAt)
                      .IsRequired();
                entity.Property(tt => tt.CreatedAt)
                      .IsRequired();
                entity.Property(tt => tt.UpdatedAt)
                      .IsRequired(false);

                //relación con Tournament
                entity.HasOne(tt => tt.Tournament)
                      .WithMany(t => t.TournamentTeams)
                      .HasForeignKey(tt =>tt.TournamentId)
                      .OnDelete(DeleteBehavior.Cascade);
                // relación con Team
                entity.HasOne(tt => tt.Team)
                      .WithMany(t => t.TournamentTeams)
                      .HasForeignKey(tt => tt.TeamId)
                      .OnDelete(DeleteBehavior.Cascade);
                // índice único compuesto: 1 equipo por torneo
                entity.HasIndex(tt => new { tt.TournamentId, tt.TeamId })
                      .IsUnique();
            });

            // Sponsor conf.
            modelBuilder.Entity<Sponsor>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(s => s.ContactEmail)
                      .IsRequired()
                      .HasMaxLength(150);
                entity.Property(s => s.Phone)
                      .IsRequired(false)
                      .HasMaxLength(25);
                entity.Property(s => s.WebsiteUrl)
                      .IsRequired(false)
                      .HasMaxLength(400);
                entity.Property(s => s.Category)
                      .IsRequired();
                entity.Property(t => t.CreatedAt)
                      .IsRequired();
                entity.Property(t => t.UpdatedAt)
                      .IsRequired(false);
            });
            //TournamentSponsor conf.
            modelBuilder.Entity<TournamentSponsor>(entity=>
            {
                entity.HasKey(ts => ts.Id);
                entity.Property(ts => ts.ContractAmount)
                      .IsRequired()
                      .HasPrecision(20,3);
                entity.Property(ts => ts.JoinedAt)
                      .IsRequired();
                entity.Property(ts => ts.CreatedAt)
                      .IsRequired();
                entity.Property(ts => ts.UpdatedAt)
                      .IsRequired(false);
                //Relacion con Tournament
                entity.HasOne(ts => ts.Tournament)
                      .WithMany(t => t.TournamentSponsors)
                      .HasForeignKey(ts=>ts.TournamentId)
                      .OnDelete(DeleteBehavior.Cascade);
                //Relacion con Sposor
                entity.HasOne(ts => ts.Sponsor)
                      .WithMany(s => s.TournamentSponsors)
                      .HasForeignKey(ts => ts.SponsorId)
                      .OnDelete(DeleteBehavior.Cascade);
                //índice único compuesto
                entity.HasIndex(ts => new { ts.TournamentId, ts.SponsorId })
                      .IsUnique();
            });

            // Match configuration

            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.MatchDate)
                      .IsRequired();
                entity.Property(m => m.Venue)
                      .HasMaxLength(150);
                entity.Property(m => m.Matchday)
                      .IsRequired();
                entity.Property(m => m.Status)
                      .IsRequired();
                entity.Property(m => m.CreatedAt)
                      .IsRequired();
                entity.Property(m => m.UpdatedAt)
                      .IsRequired(false);

                // Relación con Tournament 
                entity.HasOne(m => m.Tournament)
                      .WithMany(t => t.Matches)
                      .HasForeignKey(m => m.TournamentId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación con HomeTeam 
                entity.HasOne(m => m.HomeTeam)
                      .WithMany(t => t.HomeMatches)
                      .HasForeignKey(m => m.HomeTeamId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación con AwayTeam 
                entity.HasOne(m => m.AwayTeam)
                      .WithMany(t => t.AwayMatches)
                      .HasForeignKey(m => m.AwayTeamId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación con Referee
                entity.HasOne(m => m.Referee)
                      .WithMany(r => r.Matches)
                      .HasForeignKey(m => m.RefereeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            // MatchResult configuration
            modelBuilder.Entity<MatchResult>(entity =>
            {
                entity.HasKey(mr => mr.Id);
                entity.Property(mr => mr.HomeGoals).IsRequired();
                entity.Property(mr => mr.AwayGoals).IsRequired();
                entity.Property(mr => mr.Observations).HasMaxLength(500);
                entity.Property(mr => mr.CreatedAt).IsRequired();
                entity.Property(mr => mr.UpdatedAt).IsRequired(false);
                
                //Relacion 1:1 con match
                entity.HasOne(mr => mr.Match)
                    .WithOne(m => m.MatchResult)
                    .HasForeignKey<MatchResult>(mr => mr.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indice único de matchId
                entity.HasIndex(mr => mr.Id).IsUnique();
            });

            // Goal Configuration 
            modelBuilder.Entity<Goal>(entity => {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Minute).IsRequired();
                entity.Property(g => g.Type).IsRequired();
                entity.Property(g => g.CreatedAt).IsRequired();
                entity.Property(g => g.UpdatedAt).IsRequired(false);

                //Relacion con match 1:N
                entity.HasOne(g => g.Match)
                      .WithMany(m => m.Goals)
                      .HasForeignKey(g => g.MatchId)
                      .OnDelete(DeleteBehavior.Cascade);
                // Relacion con player 1:N
                entity.HasOne(g => g.Player)
                      .WithMany(p => p.Goals)
                      .HasForeignKey(g=>g.PlayerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            //Card Configuration
            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Minute).IsRequired();
                entity.Property(c => c.Type).IsRequired();
                entity.Property(c => c.CreatedAt).IsRequired();
                entity.Property(c => c.UpdatedAt).IsRequired(false);

                entity.HasOne(c => c.Match)
                      .WithMany(m => m.Cards)
                      .HasForeignKey(c => c.MatchId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Player)
                      .WithMany(p => p.Cards)
                      .HasForeignKey(c => c.PlayerId)
                      .OnDelete(DeleteBehavior.Restrict);

            });
            // MatchLineup conf.

            modelBuilder.Entity<MatchLineup>(entity =>
            {
                entity.HasKey(ml => ml.Id);
                entity.Property(ml => ml.IsStarter)
                      .IsRequired();
                entity.Property(ml => ml.Position)
                      .IsRequired()
                      .HasMaxLength(10);
                entity.Property(ts => ts.CreatedAt)
                      .IsRequired();
                entity.Property(ts => ts.UpdatedAt)
                      .IsRequired(false);

                // Relación 1:N con Match
                entity.HasOne(ml => ml.Match)
                      .WithMany(m => m.Lineups)
                      .HasForeignKey(ml => ml.MatchId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación 1:N con player
                entity.HasOne(ml => ml.Player)
                      .WithMany(p => p.MatchLineups)
                      .HasForeignKey(ml => ml.PlayerId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                // Evita duplicidad
                entity.HasIndex(ml => new { ml.MatchId, ml.PlayerId })
                  .IsUnique();
            });
        }
    }
}
