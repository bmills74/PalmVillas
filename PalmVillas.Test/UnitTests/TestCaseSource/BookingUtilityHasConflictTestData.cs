using PalmVillas.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmVillas.Test.UnitTests.TestCaseSource
{
    internal class BookingUtilityHasConflictTestData
    {
        internal static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(new Booking()
                {
                    VillaId = 78,
                    StartDate = DateTime.Now.ToString(),
                    EndDate = DateTime.Now.AddDays(8).ToString() // ends day after first
                });

                yield return new TestCaseData(new Booking()
                {
                    VillaId = 78,
                    StartDate = DateTime.Now.AddDays(10).ToString(), //starts day before first checkout
                    EndDate = DateTime.Now.AddDays(11).ToString()
                });
                yield return new TestCaseData(new Booking()
                {
                    VillaId = 78,
                    StartDate = DateTime.Now.AddDays(16).ToString(),
                    EndDate = DateTime.Now.AddDays(18).ToString()    //ends day after last booking starts
                });

            }
        }
    }
}
