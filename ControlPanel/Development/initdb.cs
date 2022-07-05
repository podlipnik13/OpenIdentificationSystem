using AppLibrary.Data;
using AppLibrary.Models;

namespace ControlPanel.Development;


public static class DbInitializer {

    public static void Init(OISContext context) {

        // Look for any users.
        List<User> users = null;
        
        if (!context.Users.Any()) {

            users = new() {

                new User {
                    UserName="Admin",
                    Email="",
                    Password= Cryptography.HashThis("Admin"),
                    UserGroup= UserGroup.Admin,
                    UserStatus= UserStatus.Accepted
                },
                new User {
                    UserName="Dummy1",
                    Email="",
                    Password= Cryptography.HashThis("Dummy123"),
                    UserGroup=UserGroup.Inspectee,
                    UserStatus= UserStatus.Accepted
                },
                new User {
                    UserName="Inspector1",
                    Email="",
                    Password= Cryptography.HashThis("Inspector123"),
                    UserGroup=UserGroup.Inspector,
                    UserStatus= UserStatus.Accepted,

                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }else{
            users = context.Users.ToList();
        }

        List<DocumentType> documentTypes = null;

        if (!context.DocumentTypes.Any()) {

            documentTypes = new() {
                new DocumentType {                    
                    Label="Diákigazolvány"
                },
                new DocumentType {
                    Label="sample"
                }
            };

            context.DocumentTypes.AddRange(documentTypes);
            context.SaveChanges();
        }else{
            
            documentTypes = context.DocumentTypes.ToList();
        }

        if (!context.InspectorAuthorizedDocumentTypes.Any()) {
            
            var inspectorAuthorizedDocumentTypes = new InspectorAuthorizedDocumentType[] {

                new InspectorAuthorizedDocumentType {
                    DocumentTypeId = documentTypes.First().Id,
                    UserId= users.First().Id
                }
            };

            context.InspectorAuthorizedDocumentTypes.AddRange(inspectorAuthorizedDocumentTypes);
            context.SaveChanges();
        }

        List<DocumentParameter> documentParameters = null;
        if (!context.DocumentParameters.Any()) {

            documentParameters = new() {
                new DocumentParameter {
                    Label="Kátyaszám",
                    DataType= DataType.number_int,
                    isIdentifier=true,
                    DocumentTypeId = documentTypes.First().Id
                },
                new DocumentParameter {
                    Label="Vezetéknév",
                    DataType= DataType.text,
                    isIdentifier=false,
                    DocumentTypeId = documentTypes.First().Id
                },
                new DocumentParameter {
                    Label="Keresztnév",
                    DataType= DataType.text,
                    isIdentifier=false,
                    DocumentTypeId = documentTypes.First().Id
                },
                new DocumentParameter {
                    Label="Születési idő",
                    DataType= DataType.date,
                    isIdentifier=false,
                    DocumentTypeId = documentTypes.First().Id
                },
                new DocumentParameter {
                    Label="Hely",
                    DataType=DataType.text,
                    isIdentifier=false,
                    DocumentTypeId = documentTypes.First().Id
                },
                new DocumentParameter {
                    Label="Munkarend",
                    DataType=DataType.text,
                    isIdentifier=false,
                    DocumentTypeId = documentTypes.First().Id
                },
                new DocumentParameter {
                    Label="Azonosító szám",
                    DataType=DataType.number_int,
                    isIdentifier=true,
                    DocumentTypeId = documentTypes.First().Id
                }
            };

            context.DocumentParameters.AddRange(documentParameters);
            context.SaveChanges();
        }else{
            
            documentParameters = context.DocumentParameters.ToList();

        }


        List<Document> documents = null;
        // Look for any Documents.
        if (!context.Documents.Any()) {

            documents = new() {
            
                //outdated
                new Document {
                    IssueDate = DateTime.Parse("2022-01-01").ToUniversalTime(),
                    ValidThrough = DateTime.Parse("2022-01-29").ToUniversalTime(),
                    DocumentStatus = DocumentStatus.Accepted,
                    DocumentTypeId = documentTypes.First().Id,
                    UserId = users.ElementAt(1).Id
                },
                //not accepted status
                new Document {
                    IssueDate = DateTime.Parse("2022-01-01").ToUniversalTime(),
                    ValidThrough = DateTime.Parse("2022-12-29").ToUniversalTime(),
                    DocumentStatus = DocumentStatus.Pending,
                    DocumentTypeId = documentTypes.First().Id,
                    UserId = users.ElementAt(1).Id
                },
                //incorrect type
                new Document {
                    IssueDate = DateTime.Parse("2022-01-01").ToUniversalTime(),
                    ValidThrough = DateTime.Parse("2022-12-29").ToUniversalTime(),
                    DocumentStatus = DocumentStatus.Accepted,
                    DocumentTypeId = documentTypes.ElementAt(1).Id,
                    UserId = users.ElementAt(1).Id
                },
                new Document {
                    IssueDate = DateTime.Parse("2022-01-01").ToUniversalTime(),
                    ValidThrough = DateTime.Parse("2022-12-29").ToUniversalTime(),
                    DocumentStatus = DocumentStatus.Accepted,
                    DocumentTypeId = documentTypes.First().Id,
                    UserId = users.ElementAt(1).Id
                }

            };

            context.Documents.AddRange(documents);
            context.SaveChanges();
        }else{
            
            documents = context.Documents.ToList();
        }
        
        List<DocumentParameterValue> documentParameterValues = null;

        if (!context.DocumentParameterValues.Any()) {

            documentParameterValues = new() {

                new DocumentParameterValue {
                    DocumentParameterId= documentParameters.First().Id,
                    DocumentId= documents.First().Id,
                    ParameterValue="12312321"

                },
                new DocumentParameterValue {
                    DocumentParameterId= documentParameters.ElementAt(1).Id,
                    DocumentId= documents.First().Id,
                    ParameterValue="Ilus"
                },
                new DocumentParameterValue {
                    DocumentParameterId= documentParameters.ElementAt(2).Id,
                    DocumentId=documents.First().Id,
                    ParameterValue="Bac"
                },
                new DocumentParameterValue {
                    DocumentParameterId= documentParameters.ElementAt(3).Id,
                    DocumentId=documents.First().Id,
                    ParameterValue="1991.12.19"
                },new DocumentParameterValue {
                    DocumentParameterId= documentParameters.ElementAt(4).Id,
                    DocumentId=documents.First().Id,
                    ParameterValue="Győr"
                },
                new DocumentParameterValue {
                    DocumentParameterId= documentParameters.ElementAt(5).Id,
                    DocumentId=documents.First().Id,
                },
                new DocumentParameterValue {
                    DocumentParameterId= documentParameters.ElementAt(6).Id,
                    DocumentId=documents.First().Id,
                    ParameterValue="12312312313"
                }
            };

            context.DocumentParameterValues.AddRange(documentParameterValues);
            context.SaveChanges();
        }
    
    
        // Look for any System Parameters

        if (!context.SystemParameters.Any()) {

            List<SystemParameter> systemParameters = new(){
                
                new SystemParameter {
                    Property = "ServiceProviderName",
                    Value = "MyCompany"
                },
                new SystemParameter {
                    Property = "ApiAddress",
                    Value = "127.0.0.1"
                }
            };
            
            context.SystemParameters.AddRange(systemParameters);
            context.SaveChanges();
        }
    
    
    }
}
