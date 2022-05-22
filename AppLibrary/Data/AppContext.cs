using Microsoft.EntityFrameworkCore;

namespace AppLibrary.Data;
public class OISContext : DbContext {

    #region dbSets
    public DbSet<User> Users { get; set; }
    public DbSet<InspectorAuthorizedDocumentType> InspectorAuthorizedDocumentTypes { get; set; }
    public DbSet<DocumentType> DocumentTypes { get; set; }
    public DbSet<DocumentParameter> DocumentParameters { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentParameterValue> DocumentParameterValues { get; set; }
    public DbSet<Inspection> Inspections { get; set; }
    public DbSet<SystemParameter> SystemParameters { get; set; }
    
    #endregion
    public OISContext(DbContextOptions<OISContext> options) : base(options) { }

    #region FluentAPI

    protected override void OnModelCreating(ModelBuilder modelBuilder) {

        modelBuilder.Entity<User>(e => {
            e.ToTable("users");
            e.HasKey(e => e.Id);

            e.Property(b => b.Id).UseIdentityAlwaysColumn();
            e.Property(e => e.UserName).HasMaxLength(100);
            e.Property(e => e.Email).HasMaxLength(100);
            e.Property(e => e.Password).HasMaxLength(100);
            e.Property(e => e.UserGroup);
            e.Property(e => e.UserStatus);  
        });

        modelBuilder.Entity<InspectorAuthorizedDocumentType>(e => {
            e.ToTable("inspector_authorized_document_types");

            e.HasKey(e => e.Id);

            e.HasOne(e => e.User).WithMany(p => p.InspectorAuthorizedDocumentTypes).HasForeignKey(p => p.UserId);
            e.HasOne(e => e.DocumentType).WithMany().HasForeignKey(p => p.DocumentTypeId);

        });

        modelBuilder.Entity<DocumentType>(e => {
            e.ToTable("document_types");

            e.HasKey(e => e.Id);

            e.Property(b => b.Id).UseIdentityAlwaysColumn();
            e.Property(e => e.Label).HasMaxLength(100);
        });

        modelBuilder.Entity<DocumentParameter>(e => {
            e.ToTable("document_parameters");

            e.HasKey(e => e.Id);
            e.HasOne(e => e.DocumentType).WithMany(p => p.DocumentParameters).HasForeignKey(p => p.DocumentTypeId);

            e.Property(b => b.Id).UseIdentityAlwaysColumn();
            e.Property(e => e.Label).HasMaxLength(100);
            e.Property(e => e.DataType);
            e.Property(e => e.isIdentifier);
        });

        modelBuilder.Entity<Document>(e => {
            e.ToTable("documents");

            e.HasKey(e => e.Id);
            e.HasOne(e => e.DocumentType).WithMany(p => p.Documents).HasForeignKey(p => p.DocumentTypeId);
            e.HasOne(e => e.User).WithMany(p => p.Documents).HasForeignKey(p => p.UserId);

            e.Property(b => b.Id).UseIdentityAlwaysColumn();
            e.Property(e => e.IssueDate);
            e.Property(e => e.ValidThrough);
            e.Property(e => e.DocumentStatus);
        });

        modelBuilder.Entity<DocumentParameterValue>(e => {
            e.ToTable("document_parameter_values");

            e.Property(b => b.Id).UseIdentityAlwaysColumn();
            e.HasKey(e => e.Id);
            e.HasOne(e => e.Document).WithMany(p => p.DocumentParameterValues).HasForeignKey(p => p.DocumentId);
            e.HasOne(e => e.DocumentParameter).WithMany().HasForeignKey(p => p.DocumentParameterId);

            e.Property(e => e.ParameterValue).HasMaxLength(100);
        });

        modelBuilder.Entity<Inspection>(e => {
            e.ToTable("inspections");

            e.HasKey(e => e.Id);
            e.HasOne(e => e.User).WithMany().HasForeignKey(p => p.UserId);
            e.HasOne(e => e.Inspector).WithMany().HasForeignKey(p => p.InspectorId);
            e.HasOne(e => e.Document).WithMany().HasForeignKey(p => p.DocumentId);

            e.Property(b => b.Id).UseIdentityAlwaysColumn();
            e.Property(e => e.IsValid);
            e.Property(e => e.ValidatonStartTime);
        });

        modelBuilder.Entity<SystemParameter>(e => {
            e.ToTable("system_parameters");
            e.HasKey(e => e.Id);

            e.Property(b => b.Id).UseIdentityAlwaysColumn();
            e.Property(e => e.Property).HasMaxLength(100);
            e.Property(e => e.Value).HasMaxLength(100);
        });

    }
    #endregion
}
