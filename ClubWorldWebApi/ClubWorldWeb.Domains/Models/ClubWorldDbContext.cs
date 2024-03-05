using ClubWorldWeb.Domains.Models.Config;
using ClubWorldWeb.Domains.Models.Master;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;
using ClubWorldWeb.Domains.Common;
using ClubWorldWeb.Domains.Models.Transaction;

namespace ClubWorldWeb.Domains.Models
{
    public class ClubWorldWebDbContext : IdentityDbContext<ConfigUser, Role, int>
    {
        public ClubWorldWebDbContext()
        {

        }

        public ClubWorldWebDbContext(DbContextOptions<ClubWorldWebDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(bool))
                    {
                        property.SetValueConverter(new BoolToIntConverter());
                    }
                }
            }
        }

        #region Config

        public virtual DbSet<ConfigRole> ConfigRole { get; set; }
        public virtual DbSet<ConfigUser> ConfigUser { get; set; }
        public virtual DbSet<ConfigUserRoles> ConfigUserRoles { get; set; }

        #endregion

        #region Master
        public virtual DbSet<MasMember> MasMember { get; set; }

        #endregion

        #region Transaction
        public virtual DbSet<TrnLedger> TrnLedger { get; set; }
        public virtual DbSet<TrnOnlinePayment> TrnOnlinePayment { get; set; }
        public virtual DbSet<TrnLedgerRptInfo> TrnLedgerRptInfo { get; set; }
        public virtual DbSet<TrnMemberCheckIn> TrnMemberCheckIn { get; set; }




        #endregion

        #region Report

        public virtual DbSet<RPt_MemberDashboard1> RPt_MemberDashboard1 { get; set; }
        #endregion
    }

    public class BoolToIntConverter : ValueConverter<bool, int>
    {
        public BoolToIntConverter([CanBeNull] ConverterMappingHints mappingHints = null)
            : base(
                  v => Convert.ToInt32(v),
                  v => Convert.ToBoolean(v),
                  mappingHints)
        {
        }

        public static ValueConverterInfo DefaultInfo { get; }
            = new ValueConverterInfo(typeof(bool), typeof(int), i => new BoolToIntConverter(i.MappingHints));
    }
}
