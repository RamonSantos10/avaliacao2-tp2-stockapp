using AutoMapper;
using StockApp.Application.DTOs;
using StockApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Application.Mappings
{
    public class DomainToDTOMappingProfile : Profile
    {
        public DomainToDTOMappingProfile() 
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<ProductCreateDTO, Product>();
            
            // Employee mappings
            CreateMap<Employee, EmployeeDTO>().ReverseMap();
            CreateMap<CreateEmployeeDTO, Employee>();
            CreateMap<UpdateEmployeeDTO, Employee>();
            
            // Employee Evaluation mappings
            CreateMap<EmployeeEvaluation, EmployeeEvaluationDTO>().ReverseMap();
            CreateMap<CreateEmployeeEvaluationDTO, EmployeeEvaluation>();
            CreateMap<UpdateEmployeeEvaluationDTO, EmployeeEvaluation>();
        }
    }
}
