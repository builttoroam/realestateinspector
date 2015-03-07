namespace realestateinspectorService.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class Addedproeprtytypeentity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "realestateinspector.PropertyTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Id")
                                },
                            }),
                        Name = c.String(),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion",
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Version")
                                },
                            }),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "CreatedAt")
                                },
                            }),
                        UpdatedAt = c.DateTimeOffset(precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "UpdatedAt")
                                },
                            }),
                        Deleted = c.Boolean(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Deleted")
                                },
                            }),
                    })
                    .PrimaryKey(t => t.Id,clustered:false)
                .Index(t => t.CreatedAt, clustered: true);
            
            AddColumn("realestateinspector.RealEstateProperties", "PropertyTypeId", c => c.String(maxLength: 128));
            CreateIndex("realestateinspector.RealEstateProperties", "PropertyTypeId");
            AddForeignKey("realestateinspector.RealEstateProperties", "PropertyTypeId", "realestateinspector.PropertyTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("realestateinspector.RealEstateProperties", "PropertyTypeId", "realestateinspector.PropertyTypes");
            DropIndex("realestateinspector.PropertyTypes", new[] { "CreatedAt" });
            DropIndex("realestateinspector.RealEstateProperties", new[] { "PropertyTypeId" });
            DropColumn("realestateinspector.RealEstateProperties", "PropertyTypeId");
            DropTable("realestateinspector.PropertyTypes",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "CreatedAt",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "CreatedAt" },
                        }
                    },
                    {
                        "Deleted",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Deleted" },
                        }
                    },
                    {
                        "Id",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Id" },
                        }
                    },
                    {
                        "UpdatedAt",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "UpdatedAt" },
                        }
                    },
                    {
                        "Version",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Version" },
                        }
                    },
                });
        }
    }
}
