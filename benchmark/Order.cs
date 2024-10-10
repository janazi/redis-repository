using System;

namespace Jnz.RedisRepository.Benchmark;
public class Order(int id)
{
    public int Id { get; set; } = id;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public Product Product { get; set; } = new Product(1.27 * id);
}

public class Product(double price)
{
    public double Price { get; set; } = price;
    public string Name { get; set; } = "Product " + price;
}
