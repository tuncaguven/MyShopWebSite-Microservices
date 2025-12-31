using AutoMapper;
using MyShopWebSite.Catalog.Dtos.CategoryDtos;
using MyShopWebSite.Catalog.Dtos.ProductDetailDtos;
using MyShopWebSite.Catalog.Dtos.ProductDtos;
using MyShopWebSite.Catalog.Dtos.ProductImageDtos;
using MyShopWebSite.Catalog.Entities;

namespace MyShopWebSite.Catalog.Mapper
{
    public class GeneralMapping:Profile 
    {
        public GeneralMapping()
        {
            CreateMap<Category, ResultCategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Category, UpdateCategoryDto>().ReverseMap();

            CreateMap<Product, ResultProductDto>().ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();

            CreateMap<ProductDetail, ResultProductDetailDto>().ReverseMap();
            CreateMap<ProductDetail, CreateProductDetailDto>().ReverseMap();
            CreateMap<ProductDetail, UpdateProductDetailDto>().ReverseMap();

            CreateMap<ProductImage, ResultProductImageDto>().ReverseMap();
            CreateMap<ProductImage, CreateProductImageDto>().ReverseMap();
            CreateMap<ProductImage, UpdateProductImageDto>().ReverseMap();
        }
    }
}
