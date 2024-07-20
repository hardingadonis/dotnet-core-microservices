using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository(ICatalogContext context) : IProductRepository
    {
        private readonly ICatalogContext _context = context;

        public async Task<Product> Create(Product product)
        {
            await _context.Products.InsertOneAsync(product);

            return product;
        }

        public async Task<bool> DeleteById(string id)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            var deleteResult = await _context.Products.DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _context.Products.Find(_ => true).ToListAsync();
        }

        public async Task<Product?> GetById(string id)
        {
            return await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetByName(string name)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Name, new BsonRegularExpression(name, "i"));

            return await _context.Products.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByBrand(string brandName)
        {
            var filter = Builders<Product>.Filter.Where(
                p => p.Brand != null &&
                p.Brand.Name != null &&
                p.Brand.Name.ToLower().Contains(brandName.ToLower())
                );

            return await _context.Products.Find(filter).ToListAsync();
        }

        public async Task<bool> Update(Product product)
        {
            var updateResult = await _context.Products.ReplaceOneAsync(p => p.Id == product.Id, product);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
}