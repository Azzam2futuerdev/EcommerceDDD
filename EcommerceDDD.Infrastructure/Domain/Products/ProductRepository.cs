﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDDD.Infrastructure.Domain.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly EcommerceDDDContext _context;

        public ProductRepository(EcommerceDDDContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddProduct(Product product, CancellationToken cancellationToken = default)
        {
            await _context.Products.AddAsync(product);            
        }

        public async Task AddProducts(List<Product> products, CancellationToken cancellationToken = default)
        {
            await _context.Products.AddRangeAsync(products);
        }

        public async Task<Product> GetProductById(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Product>> GetProductsByIds(List<Guid> ids, CancellationToken cancellationToken = default)
        {
            return await _context.Products.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<List<Product>> ListAllProducts(CancellationToken cancellationToken = default)
        {
            return await _context.Products.ToListAsync();
        }
    }
}