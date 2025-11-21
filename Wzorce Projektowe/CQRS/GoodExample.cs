using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DesignPatterns.CQRS.Good
{
    // ✅ GOOD: CQRS Pattern - Separate read and write models

    // ✅ WRITE SIDE - Commands

    // Command interface
    public interface ICommand { }

    public class CreateProductCommand : ICommand
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class UpdateProductPriceCommand : ICommand
    {
        public int ProductId { get; set; }
        public decimal NewPrice { get; set; }
    }

    // Write model - optimized for persistence
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // Command handlers - handle write operations
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command, CancellationToken ct = default);
    }

    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
    {
        private readonly IProductRepository _repository;

        public CreateProductCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(CreateProductCommand command, CancellationToken ct = default)
        {
            // ✅ Validation
            if (string.IsNullOrEmpty(command.Name))
                throw new ArgumentException("Product name is required");
            
            if (command.Price <= 0)
                throw new ArgumentException("Price must be positive");

            // ✅ Business logic
            var product = new Product
            {
                Name = command.Name,
                Price = command.Price,
                StockQuantity = command.StockQuantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(product, ct);
        }
    }

    public class UpdateProductPriceCommandHandler : ICommandHandler<UpdateProductPriceCommand>
    {
        private readonly IProductRepository _repository;

        public UpdateProductPriceCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(UpdateProductPriceCommand command, CancellationToken ct = default)
        {
            var product = await _repository.GetByIdAsync(command.ProductId, ct);
            if (product == null)
                throw new InvalidOperationException("Product not found");

            product.Price = command.NewPrice;
            product.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(product, ct);
        }
    }

    // ✅ READ SIDE - Queries

    // Query interface
    public interface IQuery<TResult> { }

    public class GetProductQuery : IQuery<ProductDto>
    {
        public int ProductId { get; set; }
    }

    public class GetAllProductsQuery : IQuery<List<ProductSummaryDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetLowStockProductsQuery : IQuery<List<ProductSummaryDto>>
    {
        public int Threshold { get; set; }
    }

    // Read models (DTOs) - optimized for display
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class ProductSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        // ✅ Only fields needed for listing
    }

    // Query handlers - handle read operations
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
    }

    public class GetProductQueryHandler : IQueryHandler<GetProductQuery, ProductDto>
    {
        private readonly IProductReadRepository _readRepository;

        public GetProductQueryHandler(IProductReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<ProductDto> HandleAsync(GetProductQuery query, CancellationToken ct = default)
        {
            // ✅ Optimized read from read model
            return await _readRepository.GetProductByIdAsync(query.ProductId, ct);
        }
    }

    public class GetAllProductsQueryHandler : IQueryHandler<GetAllProductsQuery, List<ProductSummaryDto>>
    {
        private readonly IProductReadRepository _readRepository;

        public GetAllProductsQueryHandler(IProductReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<List<ProductSummaryDto>> HandleAsync(GetAllProductsQuery query, CancellationToken ct = default)
        {
            // ✅ Pagination, projection
            return await _readRepository.GetProductSummariesAsync(
                query.PageNumber, 
                query.PageSize, 
                ct);
        }
    }

    public class GetLowStockProductsQueryHandler : IQueryHandler<GetLowStockProductsQuery, List<ProductSummaryDto>>
    {
        private readonly IProductReadRepository _readRepository;

        public GetLowStockProductsQueryHandler(IProductReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<List<ProductSummaryDto>> HandleAsync(GetLowStockProductsQuery query, CancellationToken ct = default)
        {
            // ✅ Optimized query
            return await _readRepository.GetLowStockProductsAsync(query.Threshold, ct);
        }
    }

    // ✅ Repository interfaces
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Product product, CancellationToken ct = default);
        Task UpdateAsync(Product product, CancellationToken ct = default);
    }

    public interface IProductReadRepository
    {
        Task<ProductDto> GetProductByIdAsync(int id, CancellationToken ct = default);
        Task<List<ProductSummaryDto>> GetProductSummariesAsync(int pageNumber, int pageSize, CancellationToken ct = default);
        Task<List<ProductSummaryDto>> GetLowStockProductsAsync(int threshold, CancellationToken ct = default);
    }

    // ✅ Usage example
    public class CqrsExample
    {
        private readonly ICommandHandler<CreateProductCommand> _createHandler;
        private readonly IQueryHandler<GetAllProductsQuery, List<ProductSummaryDto>> _getAllHandler;

        public CqrsExample(
            ICommandHandler<CreateProductCommand> createHandler,
            IQueryHandler<GetAllProductsQuery, List<ProductSummaryDto>> getAllHandler)
        {
            _createHandler = createHandler;
            _getAllHandler = getAllHandler;
        }

        public async Task RunAsync()
        {
            // ✅ Command - write operation
            var createCommand = new CreateProductCommand
            {
                Name = "Laptop",
                Price = 1200.00m,
                StockQuantity = 50
            };
            await _createHandler.HandleAsync(createCommand);

            // ✅ Query - read operation
            var getAllQuery = new GetAllProductsQuery
            {
                PageNumber = 1,
                PageSize = 10
            };
            var products = await _getAllHandler.HandleAsync(getAllQuery);
        }
    }

    // ✅ Benefits:
    // - Separate optimization of reads and writes
    // - Independent scaling
    // - Clear separation of concerns
    // - Easy to add caching on read side
    // - Different data models for different purposes
}
