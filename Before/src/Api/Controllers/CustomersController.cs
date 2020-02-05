﻿using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Dtos;
using Logic.Entities;
using Logic.Repositories;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private readonly MovieRepository _movieRepository;
        private readonly CustomerRepository _customerRepository;
        //private readonly CustomerService _customerService;

        public CustomersController(MovieRepository movieRepository, CustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            _movieRepository = movieRepository;
            //_customerService = customerService;
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(long id)
        {
            Customer customer = _customerRepository.GetById(id);
            if (customer == null)
                return NotFound();

            var dto = new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name.Value,
                Email = customer.Email.Value,
                MoneySpent = customer.MoneySpent,
                Status = customer.Status.Type.ToString(),
                StatusExpirationDate = customer.Status.ExpirationDate,
                PurchasedMovies = customer.PurchasedMovies.Select(x => new PurchasedMovieDto
                {
                    Price = x.Price,
                    ExpirationDate = x.ExpirationDate,
                    PurchaseDate = x.PurchaseDate,
                    Movie = new MovieDto
                    {
                        Id = x.Movie.Id,
                        Name = x.Movie.Name
                    }
                }).ToList()
            };

            return Json(dto);
        }

        [HttpGet]
        public JsonResult GetList()
        {
            IReadOnlyList<Customer> customers = _customerRepository.GetList();
            var dtos = customers.Select(x => new CustomerInListDto
            {
                Id = x.Id,
                Name = x.Name.Value,
                Email = x.Email.Value,
                MoneySpent = x.MoneySpent,
                Status = x.Status.ToString(),
                StatusExpirationDate = x.Status.ExpirationDate
            }).ToList();
            return Json(dtos);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateCustomerDto item)
        {
            try
            {
                Result<CustomerName> customerNameorError = CustomerName.Create(item.Name);
                Result<Email> emailorError = Email.Create(item.Email);

                Result result = Result.Combine(customerNameorError, emailorError);
                if (result.IsFailure)
                {
                    return BadRequest(result.Error);
                }

                if (_customerRepository.GetByEmail(emailorError.Value) != null)
                {
                    return BadRequest("Email is already in use: " + item.Email);
                }

                var customer = new Customer(customerNameorError.Value, emailorError.Value);
                // This makes sure that we are not violating anything.
                // like we can't create or instantiate this class without passing name and email, since it is 
                // mandotory for creating user as per business. And also prevents for interchaning email and 
                // name values. Since this is the only public construtor available to instantiate the class.
                _customerRepository.Add(customer);
                _customerRepository.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(long id, [FromBody] UpdateCustomerDto item)
        {
            try
            {
                Result<CustomerName> customerNameorError = CustomerName.Create(item.Name);
                if (customerNameorError.IsFailure)
                {
                    return BadRequest(customerNameorError.Error);
                }

                Customer customer = _customerRepository.GetById(id);
                if (customer == null)
                {
                    return BadRequest("Invalid customer id: " + id);
                }

                customer.Name = customerNameorError.Value;
                _customerRepository.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }

        [HttpPost]
        [Route("{id}/movies")]
        public IActionResult PurchaseMovie(long id, [FromBody] long movieId)
        {
            try
            {
                Movie movie = _movieRepository.GetById(movieId);
                if (movie == null)
                {
                    return BadRequest("Invalid movie id: " + movieId);
                }

                Customer customer = _customerRepository.GetById(id);
                if (customer == null)
                {
                    return BadRequest("Invalid customer id: " + id);
                }

                if (customer.PurchasedMovies.Any(x => x.Movie.Id == movie.Id && !x.ExpirationDate.IsExpired))
                {
                    return BadRequest("The movie is already purchased: " + movie.Name);
                }

                // Since we moved all the business logics to the respective entity we don't have service dependency anymore.
                //_customerService.PurchaseMovie(customer, movie);
                customer.PurchaseMovie(movie);

                _customerRepository.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }

        [HttpPost]
        [Route("{id}/promotion")]
        public IActionResult PromoteCustomer(long id)
        {
            try
            {
                Customer customer = _customerRepository.GetById(id);
                if (customer == null)
                {
                    return BadRequest("Invalid customer id: " + id);
                }

                if (customer.Status.IsAdvanced)
                {
                    return BadRequest("The customer already has the Advanced status");
                }

                //bool success = _customerService.PromoteCustomer(customer);
                bool success = customer.Promote();

                if (!success)
                {
                    return BadRequest("Cannot promote the customer");
                }

                _customerRepository.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }
    }
}
