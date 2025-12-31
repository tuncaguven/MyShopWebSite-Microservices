using AutoMapper;
using MongoDB.Driver;
using MyShopWebSite.Catalog.Dtos.ProductImageDtos;
using MyShopWebSite.Catalog.Entities;
using MyShopWebSite.Catalog.Settings;

namespace MyShopWebSite.Catalog.Services.ProductImageServices
{
    public class ProductImageService : IProductImageService
    {
        private readonly IMongoCollection<ProductImage> _productImageCollection;
        private readonly IMapper _mapper;

        public ProductImageService(IMapper mapper, IDatabaseSettings _databaseSettings)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _productImageCollection = database.GetCollection<ProductImage>(_databaseSettings.ProductImageCollectionName);
            _mapper = mapper;
        }

        public async Task CreateProductImageAsync(CreateProductImageDto createProductImageDto)
        {
            var value = _mapper.Map<ProductImage>(createProductImageDto);
            await _productImageCollection.InsertOneAsync(value);
        }

        public async Task DeleteProductImageAsync(string ProductImageId)
        {
            await _productImageCollection.DeleteOneAsync(x => x.ProductImageID == ProductImageId);
        }

        public async Task<List<ResultProductImageDto>> GetAllProductImagesAsync()
        {
            var values = await _productImageCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultProductImageDto>>(values);
        }

        public async Task<ResultProductImageDto> GetProductImageByIdAsync(string ProductImageId)
        {
            var value = await _productImageCollection.Find(x => x.ProductImageID == ProductImageId).FirstOrDefaultAsync();
            return _mapper.Map<ResultProductImageDto>(value); 
        }

        public async Task UpdateProductImageAsync(UpdateProductImageDto updateProductImageDto)
        {
            var values = _mapper.Map<ProductImage>(updateProductImageDto);

            await _productImageCollection.FindOneAndReplaceAsync(x=>x.ProductImageID == updateProductImageDto.ProductImageID,values);
        }
    }
}
