using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Driver;
using System.Configuration;

namespace MongoBaseX
{
    [BsonIgnoreExtraElements]
    public abstract class MongoBaseX<T> where T : MongoBaseX<T>
    {
        public MongoBaseX()
        {

        }

        [BsonId]
        public virtual ObjectId id { get; set; }

        public virtual void Save()
        {
            var res = MongoHelper<T>.GetHelper().Collection.ReplaceOneAsync(x => x.id == this.id, (T)this).Result;
            if (res.MatchedCount == 0)
            {
                if (id == ObjectId.Empty)
                    id = ObjectId.GenerateNewId();
                MongoHelper<T>.GetHelper().Collection.InsertOneAsync((T)this).Wait();
            }
        }

        public virtual void Delete()
        {
            MongoHelper<T>.GetHelper().Collection.DeleteOneAsync(x => x.id == this.id).Wait();
        }

        public static T FindById(ObjectId id)
        {
            return MongoHelper<T>.GetHelper().Collection.Find(x => x.id == id).ToList().FirstOrDefault();
        }

        public static List<T> Find(Expression<Func<T, bool>> filter)
        {
            return MongoHelper<T>.GetHelper().Collection.Find(filter).ToList();
        }

        public static List<T> FindAll()
        {
            return MongoHelper<T>.GetHelper().Collection.Find(x => true).ToList();
        }

        public static List<T> Where(Expression<Func<T, bool>> filter)
        {
            return Find(filter);
        }

        public static IMongoCollection<T> GetCollection()
        {
            return MongoHelper<T>.GetHelper().Collection;
        }
    }
}
