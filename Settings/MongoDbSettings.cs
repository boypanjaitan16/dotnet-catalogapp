namespace CatalogApp.Settings{
    public class MongoDbSettings{
        public string Host {get;set;}
        public string Database {get;set;}
        public string Username {get;set;}
        public string Password {get;set;}
        public int Port {get;set;}

        public string ConnectionString{
            get{
                return $"mongodb://{Username}:{Password}@{Host}:{Port}/{Database}?authSource=admin";
            }
        }
    }
}