﻿using BookStore.Domain.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BookStore.Domain.ViewModels
{
	public class ProductViewModel
	{
		public Product	Product { get; set; }

		[ValidateNever]
		public IEnumerable<SelectListItem> CategoryList { get; set; }

		[ValidateNever]
		public IEnumerable<SelectListItem> CoverTypeList { get; set; }
	}
}
