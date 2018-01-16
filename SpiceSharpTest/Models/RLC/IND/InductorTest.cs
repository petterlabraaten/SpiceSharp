﻿using System;
using System.Numerics;
using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Simulations;
using SpiceSharp.Circuits;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpiceSharpTest.Models.IND
{
    [TestClass]
    public class InductorTest : Framework
    {
        [TestMethod]
        public void LowpassRL_DC()
        {
            /*
             * Lowpass RL circuit
             * The inductor should act like an short circuit
             */
            Circuit ckt = new Circuit();
            ckt.Objects.Add(
                new Voltagesource("V1", "IN", "0", 1.0),
                new Inductor("L1", "IN", "OUT", 1e-3),
                new Resistor("R1", "OUT", "0", 1.0e3));

            // Create simulation
            OP op = new OP("op");

            // Create exports
            Func<State, double>[] exports = new Func<State, double>[1];
            op.InitializeSimulationExport += (object sender, InitializationDataEventArgs args) =>
            {
                exports[0] = op.CreateVoltageExport("OUT");
            };

            // Create references
            double[] references = { 1.0 };

            // Run test
            AnalyzeOp(op, ckt, exports, references);
        }

        [TestMethod]
        public void LowpassRL_AC()
        {
            /*
             * Lowpass RL filter in the frequency domain should have a single pole at s=-2pi*R/L
             */
            // Create circuit
            double resistance = 1;
            double inductance = 1e-3;
            Circuit ckt = new Circuit();
            ckt.Objects.Add(
                new Voltagesource("V1", "IN", "0", 0.0),
                new Inductor("L1", "IN", "OUT", inductance),
                new Resistor("R1", "OUT", "0", resistance));
            ckt.Objects["V1"].Parameters.Set("acmag", 1.0);

            // Create simulation
            AC ac = new AC("ac", "dec", 10, 0.1, 1.0e6);

            // Create exports
            Func<State, Complex>[] exports = new Func<State, Complex>[1];
            ac.InitializeSimulationExport += (object sender, InitializationDataEventArgs args) =>
            {
                exports[0] = ac.CreateAcVoltageExport("OUT");
            };

            // Create references
            Func<double, Complex>[] references = { (double f) => 1.0 / new Complex(1.0, inductance / resistance * 2 * Math.PI * f) };

            // Run test
            AnalyzeAC(ac, ckt, exports, references);
        }

        [TestMethod]
        public void LCTank_Transient()
        {
            /*
             * Test for LC tank circuit, an inductor parallel with a capacitor will resonate at a frequency of 1/(2*pi*sqrt(LC))
             */
            // Build circuit
            double capacitance = 1e-3;
            double inductance = 1e-6;
            double initialCurrent = 1e-3;
            Circuit ckt = new Circuit();
            ckt.Objects.Add(
                new Inductor("L1", "OUT", "0", inductance),
                new Capacitor("C1", "OUT", "0", capacitance)
                );
            ckt.Nodes.IC["OUT"] = 0.0;
            ckt.Objects["L1"].Parameters.Set("ic", initialCurrent);

            /*
             * WARNING: An LC tank is a circuit that oscillates and does not converge. This causes the global truncation error
             * to increase as time goes by!
             * For this reason, the absolute tolerance is made a little bit less strict.
             */
            AbsTol = 1e-9;

            // Create simulation
            Transient tran = new Transient("tran", 1e-9, 1e-3);
            tran.MaxStep = 1e-7;

            // Create exports
            Func<State, double>[] exports = new Func<State, double>[1];
            tran.InitializeSimulationExport += (object sender, InitializationDataEventArgs args) =>
            {
                exports[0] = tran.CreateExport("C1", "v");
            };

            // Create reference function
            double amplitude = Math.Sqrt(inductance / capacitance) * initialCurrent;
            double omega = 1.0 / Math.Sqrt(inductance * capacitance);
            Func<double, double>[] references = { (double t) => -amplitude * Math.Sin(omega * t) };

            // Run test
            AnalyzeTransient(tran, ckt, exports, references);
        }
    }
}
