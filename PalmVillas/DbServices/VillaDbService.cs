using Microsoft.EntityFrameworkCore;
using PalmVillas.Domain;
using PalmVillas.Models.Villas;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace PalmVillas.DbServices
{
    public interface IVillaDbService
    {
        List<Villa> ListVillas();
        Villa GetVilla(int id);
        List<Booking> GetFutureBookings(int v);
        void AddBooking(Booking booking);
        long AddVilla(Villa villa);
        long EditVilla(Villa villa);
    }

    public class VillaDbService : IVillaDbService
    {
        private readonly PalmContext db;
        public VillaDbService(PalmContext db)
        {
            this.db = db;
        }

        public void AddBooking(Booking booking)
        {
            db.Bookings.Add(booking);
            db.SaveChanges();
        }

        public long AddVilla(Villa villa)
        {
            db.Villas.Add(villa);
            db.SaveChanges();
            return villa.Id;
        }

        public long EditVilla(Villa villa)
        {
            db.Attach(villa).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            try
            {
                db.SaveChangesAsync();
                return 1;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VillaExists(villa.Id))
                {
                    return -1;
                }
                else
                {
                    throw;
                }
            }
        }

        private bool VillaExists(long id)
        {
            return (db.Villas?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public virtual List<Booking> GetFutureBookings(int villaId)
        {
            var now = DateTime.Now.ToString("yyyy-MM-dd");
            var until = DateTime.Now.AddMonths(12);
            FormattableString querystring = $"SELECT * FROM Bookings WHERE date(StartDate) BETWEEN date({now}) AND date({until}) AND VIllaId={villaId}";
            return db.Bookings                
                .FromSql(querystring).ToList();
        }

        public Villa GetVilla(int id)
        {
            return db.Villas.Find((long)id);
        }

        public List<Villa> ListVillas()
        {
            FormattableString querystring = $"SELECT * FROM Villas";
            return db.Villas
                .FromSql(querystring).ToList();
        }

    }
}
