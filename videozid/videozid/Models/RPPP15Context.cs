using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using videozid.Models;

namespace videozid.Models
{
    public partial class RPPP15Context : DbContext
    {
        public virtual DbSet<Administrator> Administrator { get; set; }
        public virtual DbSet<DhmzAcc> DhmzAcc { get; set; }
        public virtual DbSet<EkranZida> EkranZida { get; set; }
        public virtual DbSet<FerWebAcc> FerWebAcc { get; set; }
        public virtual DbSet<Kategorija> Kategorija { get; set; }
        public virtual DbSet<Korisnik> Korisnik { get; set; }
        public virtual DbSet<Koristi> Koristi { get; set; }
        public virtual DbSet<PodshemaPrikazivanja> PodshemaPrikazivanja { get; set; }
        public virtual DbSet<Prezentacija> Prezentacija { get; set; }
        public virtual DbSet<Prikazuje> Prikazuje { get; set; }
        public virtual DbSet<Sadrzaj> Sadrzaj { get; set; }
        public virtual DbSet<Sadrzi> Sadrzi { get; set; }
        public virtual DbSet<Servis> Servis { get; set; }
        public virtual DbSet<Serviser> Serviser { get; set; }
        public virtual DbSet<Servisira> Servisira { get; set; }
        public virtual DbSet<ShemaPrikazivanja> ShemaPrikazivanja { get; set; }
        public virtual DbSet<StatusUredaja> StatusUredaja { get; set; }
        public virtual DbSet<TipSadrzaja> TipSadrzaja { get; set; }
        public virtual DbSet<TipServisa> TipServisa { get; set; }
        public virtual DbSet<Uredaj> Uredaj { get; set; }
        public virtual DbSet<Videozid> Videozid { get; set; }
        public virtual DbSet<ZamjenskiUredaj> ZamjenskiUredaj { get; set; }
        public virtual DbSet<Log> Log { get; set; }

        public RPPP15Context(DbContextOptions<RPPP15Context> options) : base(options) { }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseSqlServer(@"Server=rppp.fer.hr,3000;Database=RPPP15;User Id=rppp15;Password=agro!prerada");
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdKorisnika).HasColumnName("idKorisnika");

                entity.HasOne(d => d.IdKorisnikaNavigation)
                    .WithMany(p => p.Administrator)
                    .HasForeignKey(d => d.IdKorisnika)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_idKorisnika_Administrator");
            });

            modelBuilder.Entity<DhmzAcc>(entity =>
            {
                entity.HasIndex(e => e.KorisnickoIme)
                    .HasName("UQ__DhmzAcc__46BA678E3C28AB3F")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DozvolaServer).HasColumnName("dozvolaServer");

                entity.Property(e => e.KorisnickoIme)
                    .IsRequired()
                    .HasColumnName("korisnickoIme")
                    .HasMaxLength(16);

                entity.Property(e => e.Lozinka)
                    .IsRequired()
                    .HasColumnName("lozinka")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<EkranZida>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdUredaja).HasColumnName("idUredaja");

                entity.Property(e => e.IdZida).HasColumnName("idZida");

                entity.Property(e => e.XKoord).HasColumnName("xKoord");

                entity.Property(e => e.YKoord).HasColumnName("yKoord");

                entity.HasOne(d => d.IdUredajaNavigation)
                    .WithMany(p => p.EkranZida)
                    .HasForeignKey(d => d.IdUredaja)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_IdUredaja_EkranZida");

                entity.HasOne(d => d.IdZidaNavigation)
                    .WithMany(p => p.EkranZida)
                    .HasForeignKey(d => d.IdZida)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_IdZida_EkranZida");
            });

            modelBuilder.Entity<FerWebAcc>(entity =>
            {
                entity.HasIndex(e => e.KorisnickoIme)
                    .HasName("UQ__FerWebAc__46BA678E54D43552")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DozvolaServer).HasColumnName("dozvolaServer");

                entity.Property(e => e.KorisnickoIme)
                    .IsRequired()
                    .HasColumnName("korisnickoIme")
                    .HasMaxLength(16);

                entity.Property(e => e.Lozinka)
                    .IsRequired()
                    .HasColumnName("lozinka")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Kategorija>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Vrsta)
                    .IsRequired()
                    .HasColumnName("vrsta")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Korisnik>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("UQ__Korisnik__AB6E61643738C05B")
                    .IsUnique();

                entity.HasIndex(e => e.KorisnickoIme)
                    .HasName("UQ__Korisnik__46BA678EF3DB7EB8")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DhmzId).HasColumnName("dhmzId");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(30);

                entity.Property(e => e.FerId).HasColumnName("ferId");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasColumnName("ime")
                    .HasMaxLength(16);

                entity.Property(e => e.KorisnickoIme)
                    .IsRequired()
                    .HasColumnName("korisnickoIme")
                    .HasMaxLength(16);

                entity.Property(e => e.Lozinka)
                    .IsRequired()
                    .HasColumnName("lozinka")
                    .HasMaxLength(20);

                entity.Property(e => e.Prezime)
                    .IsRequired()
                    .HasColumnName("prezime")
                    .HasMaxLength(16);

                entity.HasOne(d => d.Dhmz)
                    .WithMany(p => p.Korisnik)
                    .HasForeignKey(d => d.DhmzId)
                    .HasConstraintName("fk_dhmzId_Korisnik");

                entity.HasOne(d => d.Fer)
                    .WithMany(p => p.Korisnik)
                    .HasForeignKey(d => d.FerId)
                    .HasConstraintName("fk_ferId_Korisnik");
            });

            modelBuilder.Entity<Koristi>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Do).HasColumnName("do");

                entity.Property(e => e.IdPodsheme).HasColumnName("idPodsheme");

                entity.Property(e => e.IdPrezentacije).HasColumnName("idPrezentacije");

                entity.Property(e => e.Od).HasColumnName("od");

                entity.HasOne(d => d.IdPodshemeNavigation)
                    .WithMany(p => p.Koristi)
                    .HasForeignKey(d => d.IdPodsheme)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_IdPodsheme_Koristi");

                entity.HasOne(d => d.IdPrezentacijeNavigation)
                    .WithMany(p => p.Koristi)
                    .HasForeignKey(d => d.IdPrezentacije)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_IdPrezentacije_Koristi");
            });

            modelBuilder.Entity<PodshemaPrikazivanja>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasColumnName("naziv")
                    .HasMaxLength(25);

                entity.Property(e => e.Opis)
                    .HasColumnName("opis")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Prezentacija>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdKategorije).HasColumnName("idKategorije");

                entity.Property(e => e.IdSadrzaja).HasColumnName("idSadrzaja");

                entity.Property(e => e.Sirina).HasColumnName("sirina");

                entity.Property(e => e.Visina).HasColumnName("visina");

                entity.Property(e => e.XKoord).HasColumnName("xKoord");

                entity.Property(e => e.YKoord).HasColumnName("yKoord");

                entity.HasOne(d => d.IdKategorijeNavigation)
                    .WithMany(p => p.Prezentacija)
                    .HasForeignKey(d => d.IdKategorije)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_idKategorije_Prezentacija");

                entity.HasOne(d => d.IdSadrzajaNavigation)
                    .WithMany(p => p.Prezentacija)
                    .HasForeignKey(d => d.IdSadrzaja)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_idSadrzaja_Prezentacija");
            });

            modelBuilder.Entity<Prikazuje>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Do)
                    .HasColumnName("do")
                    .HasColumnType("date");

                entity.Property(e => e.IdSheme).HasColumnName("idSheme");

                entity.Property(e => e.IdZida).HasColumnName("idZida");

                entity.Property(e => e.Od)
                    .HasColumnName("od")
                    .HasColumnType("date");

                entity.HasOne(d => d.IdShemeNavigation)
                    .WithMany(p => p.Prikazuje)
                    .HasForeignKey(d => d.IdSheme)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_IdSheme_Prikazuje");

                entity.HasOne(d => d.IdZidaNavigation)
                    .WithMany(p => p.Prikazuje)
                    .HasForeignKey(d => d.IdZida)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_IdZida_Prikazuje");
            });

            modelBuilder.Entity<Sadrzaj>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdAutora).HasColumnName("idAutora");

                entity.Property(e => e.IdOdobrenOd).HasColumnName("idOdobrenOD");

                entity.Property(e => e.IdTipa).HasColumnName("idTipa");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasColumnName("ime")
                    .HasMaxLength(50);

                entity.Property(e => e.JeOdobren)
                    .HasColumnName("jeOdobren")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Opis)
                    .IsRequired()
                    .HasColumnName("opis")
                    .HasMaxLength(50);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("url")
                    .HasMaxLength(2083);

                entity.HasOne(d => d.IdAutoraNavigation)
                    .WithMany(p => p.SadrzajIdAutoraNavigation)
                    .HasForeignKey(d => d.IdAutora)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_idAutora_Sadrzaj");

                entity.HasOne(d => d.IdOdobrenOdNavigation)
                    .WithMany(p => p.SadrzajIdOdobrenOdNavigation)
                    .HasForeignKey(d => d.IdOdobrenOd)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_idOdobrenOd_Sadrzaj");

                entity.HasOne(d => d.IdTipaNavigation)
                    .WithMany(p => p.Sadrzaj)
                    .HasForeignKey(d => d.IdTipa)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_idTipa_Sadrzaj");
            });

            modelBuilder.Entity<Sadrzi>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Do).HasColumnName("do");

                entity.Property(e => e.IdPodsheme).HasColumnName("idPodsheme");

                entity.Property(e => e.IdSheme).HasColumnName("idSheme");

                entity.Property(e => e.Od).HasColumnName("od");

                entity.HasOne(d => d.IdPodshemeNavigation)
                    .WithMany(p => p.Sadrzi)
                    .HasForeignKey(d => d.IdPodsheme)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_IdPodsheme_Sadrzi");

                entity.HasOne(d => d.IdShemeNavigation)
                    .WithMany(p => p.Sadrzi)
                    .HasForeignKey(d => d.IdSheme)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_IdSheme_Sadrzi");
            });

            modelBuilder.Entity<Servis>(entity =>
            {
                entity.HasIndex(e => e.ZiroRacun)
                    .HasName("UQ__Servis__04037E8476CC5A3B")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasColumnName("ime")
                    .HasMaxLength(20);

                entity.Property(e => e.Opis)
                    .HasColumnName("opis")
                    .HasMaxLength(200);

                entity.Property(e => e.ZiroRacun)
                    .IsRequired()
                    .HasColumnName("ziroRacun")
                    .HasMaxLength(21);
            });

            modelBuilder.Entity<Serviser>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdServis).HasColumnName("idServis");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasColumnName("ime")
                    .HasMaxLength(10);

                entity.Property(e => e.Opis)
                    .HasColumnName("opis")
                    .HasMaxLength(100);

                entity.Property(e => e.Prezime)
                    .IsRequired()
                    .HasColumnName("prezime")
                    .HasMaxLength(15);

                entity.HasOne(d => d.IdServisNavigation)
                    .WithMany(p => p.Serviser)
                    .HasForeignKey(d => d.IdServis)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_idServis_Serviser");
            });

            modelBuilder.Entity<Servisira>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdServis).HasColumnName("idServis");

                entity.Property(e => e.IdUredaj).HasColumnName("idUredaj");

                entity.HasOne(d => d.IdServisNavigation)
                    .WithMany(p => p.Servisira)
                    .HasForeignKey(d => d.IdServis)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_idServis_Servisira");

                entity.HasOne(d => d.IdUredajNavigation)
                    .WithMany(p => p.Servisira)
                    .HasForeignKey(d => d.IdUredaj)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_idUredaj_Servisira");
            });

            modelBuilder.Entity<ShemaPrikazivanja>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasColumnName("naziv")
                    .HasMaxLength(25);

                entity.Property(e => e.Opis)
                    .HasColumnName("opis")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<StatusUredaja>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasColumnName("naziv")
                    .HasMaxLength(25);
            });

            modelBuilder.Entity<TipSadrzaja>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasColumnName("ime")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TipServisa>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdServis).HasColumnName("idServis");

                entity.Property(e => e.Tip)
                    .IsRequired()
                    .HasColumnName("tip")
                    .HasMaxLength(40);

                entity.HasOne(d => d.IdServisNavigation)
                    .WithMany(p => p.TipServisa)
                    .HasForeignKey(d => d.IdServis)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_TipServisa_Servisira");
            });

            modelBuilder.Entity<Uredaj>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AktualnaCijena).HasColumnName("aktualnaCijena");

                entity.Property(e => e.DatumNabavke)
                    .HasColumnName("datumNabavke")
                    .HasColumnType("date");

                entity.Property(e => e.IdNadredeneKomponente).HasColumnName("idNadredeneKomponente");

                entity.Property(e => e.IdStatusa).HasColumnName("idStatusa");

                entity.Property(e => e.IdZida).HasColumnName("idZida");

                entity.Property(e => e.NabavnaCijena).HasColumnName("nabavnaCijena");

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasColumnName("naziv")
                    .HasMaxLength(50);

                entity.HasOne(d => d.IdNadredeneKomponenteNavigation)
                    .WithMany(p => p.InverseIdNadredeneKomponenteNavigation)
                    .HasForeignKey(d => d.IdNadredeneKomponente)
                    .HasConstraintName("fk_IdNadređeneKomponente_Uredaj");

                entity.HasOne(d => d.IdStatusaNavigation)
                    .WithMany(p => p.Uredaj)
                    .HasForeignKey(d => d.IdStatusa)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_Status_Uredaj");

                entity.HasOne(d => d.IdZidaNavigation)
                    .WithMany(p => p.Uredaj)
                    .HasForeignKey(d => d.IdZida)
                    .HasConstraintName("fk_IdZida_Uredaj");
            });

            modelBuilder.Entity<Videozid>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Lokacija)
                    .IsRequired()
                    .HasColumnName("lokacija")
                    .HasMaxLength(50);

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasColumnName("naziv")
                    .HasMaxLength(25);

                entity.Property(e => e.Sirina).HasColumnName("sirina");

                entity.Property(e => e.Visina).HasColumnName("visina");
            });

            modelBuilder.Entity<ZamjenskiUredaj>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdUredaja).HasColumnName("idUredaja");

                entity.Property(e => e.IdZamjenaZa).HasColumnName("idZamjenaZa");

                entity.HasOne(d => d.IdUredajaNavigation)
                    .WithMany(p => p.ZamjenskiUredajIdUredajaNavigation)
                    .HasForeignKey(d => d.IdUredaja)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_IdUredaja_ZamjenskiUredaj");

                entity.HasOne(d => d.IdZamjenaZaNavigation)
                    .WithMany(p => p.ZamjenskiUredajIdZamjenaZaNavigation)
                    .HasForeignKey(d => d.IdZamjenaZa)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_IdZamjenaZa_ZamjenskiUredaj");
            });

            modelBuilder.Entity<Log>(entity =>
           {
               entity.Property(e => e.Id).HasColumnName("id");
               entity.Property(e => e.Level).HasColumnName("Level");
               entity.Property(e => e.Message).HasColumnName("Message");
               entity.Property(e => e.Logged).HasColumnName("Logged");
               entity.Property(e => e.Callsite).HasColumnName("Callsite");
               entity.Property(e => e.Logger).HasColumnName("Logger");
               entity.Property(e => e.Exception).HasColumnName("Exception");
               entity.Property(e => e.Application).HasColumnName("Application");
           });
        }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseSqlServer(@"Server=rppp.fer.hr,3000;Database=RPPP15;User Id=rppp15;Password=agro!prerada");
        }*/

    }
}