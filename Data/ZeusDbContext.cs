using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Domain;

namespace Zeus.Data
{
    public class ZeusDbContext : DbContext
    {
        public virtual DbSet<Pergunta> Perguntas { get; set; }
        public virtual DbSet<Resposta> Respostas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ZeusDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new SqlConnectionStringBuilder
                {
                    ApplicationName = "ZEUS",
                    DataSource = "(localdb)\\MSSQLLocalDB",
                    InitialCatalog = "zeus",
                    ConnectRetryCount = 3,
                    ConnectTimeout = 30,
                    MinPoolSize = 5
                };

                optionsBuilder.UseSqlServer(builder.ConnectionString);
            }
        }
    }

    public class PerguntaConfig : IEntityTypeConfiguration<Pergunta>
    {
        public void Configure(EntityTypeBuilder<Pergunta> builder)
        {
            builder.ToTable("Pergunta");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Descricao);

            builder.HasMany(x => x.Respostas)
                   .WithOne(x => x.Pergunta)
                   .HasForeignKey(x => x.PerguntaId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class RespostaConfig : IEntityTypeConfiguration<Resposta>
    {
        public void Configure(EntityTypeBuilder<Resposta> builder)
        {
            builder.ToTable("Resposta");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Descricao);
            builder.Property(x => x.PerguntaId);
        }
    }
}
