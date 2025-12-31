using AutoMapper;
using MongoDB.Driver;
using MyShopWebSite.Catalog.Dtos.ProductDtos;
using MyShopWebSite.Catalog.Entities;
using MyShopWebSite.Catalog.Settings;

namespace MyShopWebSite.Catalog.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMapper _mapper;

        public ProductService(IMapper mapper, IDatabaseSettings _databaseSettings)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _productCollection = database.GetCollection<Product>(_databaseSettings.ProductCollectionName);
            _mapper = mapper;
        }

        public async Task CreateProductAsync(CreateProductDto createProductDto)
        {
            var value = _mapper.Map<Product>(createProductDto);
            await _productCollection.InsertOneAsync(value);
        }

        public async Task DeleteProductAsync(string ProductId)
        {
            await _productCollection.DeleteOneAsync(x => x.ProductID == ProductId);
        }

        public async Task<List<ResultProductDto>> GetAllProductsAsync()
        {
            var values = await _productCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultProductDto>>(values);
        }

        public async Task<ResultProductDto> GetProductByIdAsync(string ProductId)
        {
            var value = await _productCollection.Find(x => x.ProductID == ProductId).FirstOrDefaultAsync();
            return _mapper.Map<ResultProductDto>(value); 
        }

        public async Task UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            var values = _mapper.Map<Product>(updateProductDto);

            await _productCollection.FindOneAndReplaceAsync(x=>x.ProductID == updateProductDto.ProductID,values);
        }
    }
}
