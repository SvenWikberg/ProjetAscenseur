/*
 * Sven Wikberg et Seb Mata
 * 2017
 * 
 * Connexion machine/USB Client EDP Module : USB/microUSB
 * 42BYG StepperMotor/Stepper L6470 Module : cable bleu/1B - cable rouge/2B - cable vert/1A - cable noir/2A
 * Breakout TB10/Alimentation 12vc : PWR/+ - GND/-
 * Sensor 2Y0A21/Breakout TB10 : cable jaune/p3 - cable rouge/p2 - cable noir/p10 
 * FEZ Spider Mainboard/USB Client EDP Module : connecteur 1
 * 
*/
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GTM = Gadgeteer.Modules;

namespace testAscenseur
{
    public class Program
    {
        const double DEPART_CHARIOT = 0.255; // direction moteur = direction reverse
        const double ARRIVEE_CHARIOT = 0.94;
        const double DEPART_POTENTIOMETRE = 0;
        const double ARRIVEE_POTENTIOMETRE = 1;
        const int MAX_SPEED_CHARIOT = 30000;

        static uint speed;
        static double sensorValue;
        static double potentioMeterValue;

        public static void Main()
        {
            GHIElectronics.Gadgeteer.FEZSpider mainBoard = new GHIElectronics.Gadgeteer.FEZSpider();
            GTM.GHIElectronics.StepperL6470 motorStepper = new GTM.GHIElectronics.StepperL6470(6);
            AnalogInput potentioMeter = new AnalogInput(Cpu.AnalogChannel.ANALOG_6);
            AnalogInput sensor = new AnalogInput(Cpu.AnalogChannel.ANALOG_7);
            Gadgeteer.Modules.GHIElectronics.StepperL6470.Direction direction = new GTM.GHIElectronics.StepperL6470.Direction();

            speed = 0;
            sensorValue = 0;
            potentioMeterValue = 0;

            motorStepper.Run(StepperL6470.Direction.Forward, 0);

            /*lis les valeurs du senseur et du potentiomètre et 
             * gère le déplacement de l'ascenceur dans la bonne direction et 
             * à la bonne vitesse pour que les deux valeurs se rejoignent*/
            while (true)
            {
                //transforme la valeur du senseur et du potentiomètre en pourcentage sur la distance totale
                sensorValue = 1 - ValueToRatio(DEPART_CHARIOT, ARRIVEE_CHARIOT, sensor.Read());
                potentioMeterValue = ValueToRatio(DEPART_POTENTIOMETRE, ARRIVEE_POTENTIOMETRE, potentioMeter.Read());

                if (sensorValue < potentioMeterValue)
                {
                    direction = GTM.GHIElectronics.StepperL6470.Direction.Reverse;
                    speed = (uint)((potentioMeterValue - sensorValue) * MAX_SPEED_CHARIOT);
                }
                else
                {
                    direction = GTM.GHIElectronics.StepperL6470.Direction.Forward;
                    speed = (uint)((sensorValue - potentioMeterValue) * MAX_SPEED_CHARIOT);
                }
                //Lance l'action en fonction des directions et vitesses prédéterminées
                motorStepper.Run(direction, speed);

                Debug.Print(sensor.Read().ToString());
            }
        }

        /// <summary>
        /// Converti une valeur actuelle en pourcentage
        /// </summary>
        /// <param name="min">la valeur minimum possible</param>
        /// <param name="max">la valeur maximum possible</param>
        /// <param name="value">position actuelle sur l'axe entre la valeur maximum et la valeur minimum</param>
        /// <returns></returns>
        public static double ValueToRatio(double min, double max, double value)
        {
            /*
            if (value<0)
            value = value * -1;
            if (min < 0)
                min = min * -1;
            if (max < 0)
                max = max * -1;
            */

            return (value - min) / (max - min);
        }

        /// <summary>
        /// Converti un pourcentage en valeur actuelle
        /// </summary>
        /// <param name="min">la valeur minimum possible</param>
        /// <param name="max">la valeur maximum possible</param>
        /// <param name="ratio">le poucentage à convertir en valeur actuelle</param>
        /// <returns></returns>
        public static double RatioToValue(double min, double max, double ratio)
        {
            /*
            if (value<0)
            ratio = ratio * -1;
            if (min < 0)
                min = min * -1;
            if (max < 0)
                max = max * -1;
            */
            return (ratio * (max - min)) + min;
        }
    }
}