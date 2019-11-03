﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Order
    /// </summary>
    public class OrderModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Id do cliente
        /// </summary>
        [Required]
        public int IdCustomer { get; set; }
        /// <summary>
        /// Display Id do pedido
        /// </summary>
        public string DispId { get; set; }
        /// <summary>
        /// Subtotal
        /// </summary>
        [Required]
        public decimal Subtotal { get; set; }
        /// <summary>
        /// Desconto
        /// </summary>
        [Required]
        public decimal Discount { get; set; }
        /// <summary>
        /// Total
        /// </summary>
        [Required]
        public decimal Total { get; set; }
        /// <summary>
        /// Quantidade de itens do pedido
        /// </summary>
        [Required]
        public int TotalItems { get; set; }
        /// <summary>
        /// IP de onde foi realizado o pedido
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// Id do pedido no wirecard
        /// </summary>
        public string WirecardId { get; set; }
        /// <summary>
        /// Status do pedido
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Forma de Pagamento
        /// </summary>
        public string PaymentType { get; set; }
        /// <summary>
        /// Data da realização do pedido
        /// </summary>
        public DateTime Created { get; set; }

        public List<OrderItemModel> Items { get; set; }
        public WirecardPaymentModel Payment { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public OrderModel() {}

        /// <summary>
        /// Construtor que recebe um Order e popula os atributos
        /// </summary>
        /// <param name="order"></param>
        public OrderModel(Order order)
        {
            this.Id = order.Id;
            this.IdCustomer = order.IdCustomer;
            this.DispId = order.DispId;
            this.Subtotal = order.Subtotal;
            this.Discount = order.Discount;
            this.Total = order.Total;
            this.TotalItems = order.TotalItems;
            this.IP = order.IP;
            this.WirecardId = order.WirecardId;
            this.Status = order.Status;
            this.PaymentType = order.PaymentType;
            this.Created = order.Created;
            this.Items = new List<OrderItemModel>();
            if(order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                    this.Items.Add(new OrderItemModel(item));
            }
            if(order.WirecardPayments != null)
            {
                var payment = order.WirecardPayments.OrderByDescending(p => p.Created).FirstOrDefault();
                if (payment != null)
                    this.Payment = new WirecardPaymentModel(payment);
            }
        }

        /// <summary>
        /// Retorna um objeto Order com as informações
        /// </summary>
        /// <returns></returns>
        public Order GetEntity()
        {
            var ret = new Order()
            {
                Id = this.Id,
                IdCustomer = this.IdCustomer,
                DispId = this.DispId,
                Subtotal = this.Subtotal,
                Discount = this.Discount,
                Total = this.Total,
                TotalItems = this.TotalItems,
                IP = this.IP,
                WirecardId = this.WirecardId,
                Status = this.Status,
                PaymentType = this.PaymentType,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };

            if (this.Items != null)
            {
                foreach (var item in this.Items)
                    ret.OrderItems.Add(item.GetEntity());
            }

            return ret;
        }
    }

    public class OrderItemModel
    {
        /// <summary>
        /// Id do item
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id do curso
        /// </summary>
        public int IdCourse { get; set; }
        /// <summary>
        /// Nome do curso
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Valor do Curso
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Voucher
        /// </summary>
        public string Voucher { get; set; }

        public OrderItemModel() { }
        public OrderItemModel(OrderItem orderItem) 
        {
            this.Id = orderItem.Id;
            this.IdCourse = orderItem.IdCourse;
            this.Name = orderItem.Name;
            this.Price = orderItem.Price;
            this.Voucher = orderItem.Voucher;
        }

        public OrderItem GetEntity()
        {
            return new OrderItem()
            {
                Id = this.Id,
                IdCourse = this.IdCourse,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                Name = this.Name,
                Price = this.Price
            };
        }
    }

    public class OrderWirecardInfo
    {
        /// <summary>
        /// Id do pedido
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id do pedido no wirecard
        /// </summary>
        public string WirecardId { get; set; }
        /// <summary>
        /// Status do pedido
        /// </summary>
        public string Status { get; set; }
    }
}