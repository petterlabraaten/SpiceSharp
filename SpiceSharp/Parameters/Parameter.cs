﻿using System;

namespace SpiceSharp
{
    /// <summary>
    /// Base class for parameters
    /// Parameters are objects that contain a double value, and that have some basic manipulations. They
    /// also make it easier to be referenced by simulations, sweeps and other features.
    /// </summary>
    public abstract class Parameter : BaseParameter
    {
        /// <summary>
        /// Gets or sets the value of the parameter
        /// </summary>
        public abstract double Value { get; set; }

        /// <summary>
        /// Copy from another parameter
        /// </summary>
        /// <param name="source">Source</param>
        public override void CopyFrom(BaseParameter source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source is Parameter p)
                Value = p.Value;
            else
                throw new CircuitException("Cannot copy: source is not a Parameter");
        }

        /// <summary>
        /// Implicit conversion for a parameter to a double
        /// </summary>
        /// <param name="parameter">Parameter</param>
        public static implicit operator double(Parameter parameter) => parameter?.Value ?? double.NaN;

        /// <summary>
        /// Convert to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Parameter {0}".FormatString(Value);
        }
    }
}
