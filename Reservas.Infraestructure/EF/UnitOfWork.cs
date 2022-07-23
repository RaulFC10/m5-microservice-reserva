﻿using MediatR;
using Reservas.Domain.Repositories;
using Reservas.Infraestructure.EF.Contexts;
using ShareKernel.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Reservas.Infraestructure.EF {
	public class UnitOfWork : IUnitOfWork {
		private readonly WriteDbContext _context;
		private readonly IMediator _mediator;

		public UnitOfWork(WriteDbContext context, IMediator mediator) {
			_context = context;
			_mediator = mediator;
		}
		public async Task guardar() {
			await _context.SaveChangesAsync();
		}
		public async Task Commit() {
			//Publicar eventos de dominio
			var domainEvents = _context.ChangeTracker.Entries<Entity<Guid>>()
				.Select(x => x.Entity.DomainEvents)
				.SelectMany(x => x)
				.ToArray();

			foreach (var @event in domainEvents)
			{
				await _mediator.Publish(@event);
			}

			await _context.SaveChangesAsync();
		}
	}
}
