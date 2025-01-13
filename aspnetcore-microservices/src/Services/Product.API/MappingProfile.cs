﻿using AutoMapper;
using Product.API.Entities;
using Shared.DTOs;
using Infrastructure.Mappings;
using Shared.DTOs.Product;

namespace Product.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CatalogProduct, ProductDto>();
            CreateMap<CreateProductDto, CatalogProduct>(); // .ReverseMap()
            CreateMap<UpdateProductDto, CatalogProduct>()
                .IgnoreAllNonExisting();

        }
    }
}
