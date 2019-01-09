using AutoMapper;
using Library.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            // CreateMap<TSource, TDestination>()
            CreateMap<AddEditBookViewModel, Book>();


           // CreateMap<Book,AddEditBookViewModel>();
        }
    }
}
