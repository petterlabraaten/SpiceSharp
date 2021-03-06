﻿using SpiceSharp.Entities;
using SpiceSharp.General;
using SpiceSharp.ParameterSets;
using SpiceSharp.Simulations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace SpiceSharp.Behaviors
{
    /// <summary>
    /// A container for behaviors
    /// </summary>
    /// <seealso cref="IParameterSetCollection"/>
    public interface IBehaviorContainer :
        ITypeSet<IBehavior>,
        IParameterSetCollection
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name of the behavior container.
        /// </value>
        /// <remarks>
        /// This is typically the name of the entity that creates the behaviors in this container.
        /// </remarks>
        string Name { get; }

        /// <summary>
        /// Starts building the behaviors for the behavior container.
        /// </summary>
        /// <typeparam name="TContext">The type of binding context.</typeparam>
        /// <param name="simulation">The simulation.</param>
        /// <param name="context">The binding context.</param>
        /// <returns>The behavior container builder.</returns>
        IBehaviorContainerBuilder<TContext> Build<TContext>(ISimulation simulation, TContext context)
            where TContext : IBindingContext;
    }
}
