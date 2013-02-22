using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SampleRestService.Resources;

namespace SampleRestService.DataAccess
{
    public class ProductRepository
    {
        // in-memory data store
        private static readonly ConcurrentDictionary<int, Product> productStore = new ConcurrentDictionary<int, Product>(new Dictionary<int, Product>
        {
            { 1, new Product { ID = 1, Name = "Milk", Price = 3.99m, Added = DateTime.Now, InStock = true } },
            { 2, new Product { ID = 2, Name = "Bread", Price = 1.99m, Added = DateTime.Now, InStock = true } },
            { 3, new Product { ID = 3, Name = "Coffee machine", Price = 299.99m, Added = DateTime.Now, InStock = false } },
            { 4, new Product { ID = 4, Name = "AA batteries (12-pack)", Price = 7.99m, Added = DateTime.Now, InStock = true } },
            { 5, new Product { ID = 5, Name = "Barbeque coal", Price = 12.99m, Added = DateTime.Now, InStock = false } }
        });

        private static readonly object syncRoot = new object();

        public virtual bool Contains(int id)
        {
            return productStore.ContainsKey(id);
        }

        public virtual IQueryable<Product> GetAll()
        {
            var products = productStore.Values.OrderBy(p => p.ID);
            var clonedProducts = new List<Product>();

            foreach (var product in products)
            {
                clonedProducts.Add(Clone(product));
            }

            return clonedProducts.AsQueryable();
        }

        public virtual void Add(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }

            if (product.ID != 0)
            {
                throw new ArgumentException("A new product must not have an ID", "product");
            }

            lock (syncRoot)
            {
                product.ID = productStore.Keys.DefaultIfEmpty().Max(key => key) + 1;

                if (!productStore.TryAdd(product.ID, product))
                {
                    throw new InvalidOperationException("Duplicate product ID provided");
                }

                product.Added = DateTime.UtcNow;
            }
        }

        public virtual void Update(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }

            if (product.ID <= 0)
            {
                throw new ArgumentException("Product ID must be greater than 0", "product");
            }

            lock (syncRoot)
            {
                Product storedProduct;

                if (!productStore.TryGetValue(product.ID, out storedProduct))
                {
                    throw new InvalidOperationException("Product could not be updated because it cannot be found");
                }

                if (!productStore.TryUpdate(product.ID, product, storedProduct))
                {
                    throw new InvalidOperationException("There was a concurrency problem updating the product");
                }
            }
        }

        public virtual void UpdateInStockStatus(int productId, bool inStock)
        {
            lock (syncRoot)
            {
                Product storedProduct;

                if (!productStore.TryGetValue(productId, out storedProduct))
                {
                    throw new InvalidOperationException("Product could not be updated because it cannot be found");
                }

                storedProduct.InStock = inStock;
            }
        }

        public Task AddAsync(Product product)
        {
            return Task.Factory.StartNew(p => Add((Product) p), product);
        }
   
        public Task UpdateAsync(Product product)
        {
            return Task.Factory.StartNew(p => Update((Product) p), product);
        }

        public virtual bool Delete(int productId)
        {
            Product storedProduct;

            return productStore.TryRemove(productId, out storedProduct);
        }

        private static Product Clone(Product product)
        {
            if (product == null)
            {
                return null;
            }

            return new Product
            {
                ID = product.ID,
                Name = product.Name,
                Price = product.Price,
                Added = product.Added,
                InStock = product.InStock
            };
        }
    }
}