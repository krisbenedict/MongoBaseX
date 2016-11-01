using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using System.Configuration;
using MongoDB.Bson;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace MongoBaseX
{
    public class MongoHelper<T> where T : class
    {
        private IMongoDatabase _db;
        private IMongoCollection<T> _collection;

        public MongoHelper(IMongoDatabase db, string collectionName)
        {
            _db = db;
            _collection = _db.GetCollection<T>(collectionName);
        }

        public IMongoCollection<T> Collection
        {
            get
            {
                return _collection;
            }
        }

        public static MongoHelper<T> GetHelper()
        {
            return new MongoHelper<T>(new MongoClient(MongoHelper.connString).GetDatabase(new MongoUrl(MongoHelper.connString).DatabaseName), typeof(T).Name);
        }

        public async Task<T> FindOne(Expression<Func<T, bool>> filter)
        {
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T> FindOne(Expression<Func<T, bool>> filter, SortDefinition<T> sort)
        {
            return await _collection.Find(filter).Sort(sort).FirstOrDefaultAsync();
        }

        public async Task<List<T>> Find(Expression<Func<T, bool>> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<T>> Find()
        {
            return await _collection.Find(x => true).ToListAsync();
        }

        public async Task Insert(T document)
        {
            await _collection.InsertOneAsync(document);
        }

        public async Task<bool> Exists(Expression<Func<T, bool>> filter)
        {
            return (await _collection.CountAsync(filter)) != 0;
        }

        public async Task Update(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, UpdateOptions options = null)
        {
            await _collection.UpdateOneAsync(filter, update, options);
        }

        public async Task Replace(Expression<Func<T, bool>> filter, T replacement, UpdateOptions options = null)
        {
            await _collection.ReplaceOneAsync(filter, replacement, options);
        }

        public async Task Remove(Expression<Func<T, bool>> filter)
        {
            await _collection.DeleteManyAsync(filter);
        }
    }

    public class MongoHelper
    {
        public static string connString { get; set; }
    }
}
