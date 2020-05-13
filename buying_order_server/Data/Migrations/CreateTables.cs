using FluentMigrator;

namespace buying_order_server.Data.Migrations
{
    [Migration(0)]
    public class CreateTables : Migration
    {

        public override void Up()
        {
            Create.Table("OrderNotification")
                .WithColumn("BuyingOrderId").AsInt32().PrimaryKey()
                .WithColumn("ProviderId").AsInt32()
                .WithColumn("Timestamp").AsDateTime()
                .WithColumn("Sent").AsBoolean()
                .WithColumn("OrderDate").AsDate()
                .WithColumn("EstimatedOrderDate").AsDate()
                .WithColumn("ProviderEmail").AsString()
                .WithColumn("EmployeeEmail").AsString();

            Create.Table("AppConfiguration")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("AppEmailName").AsString()
                .WithColumn("AppEmailUser").AsString()
                .WithColumn("AppEmailPassword").AsString()
                .WithColumn("AppSMTPAddress").AsString()
                .WithColumn("AppSMTPPort").AsInt32()
                .WithColumn("AppEmailFrom").AsString()
                .WithColumn("AppEmailSubject").AsString()
                .WithColumn("AppCronPattern").AsString()
                .WithColumn("AppCronTimezone").AsString()
                .WithColumn("AppNotificationTriggerDelta").AsInt32()
                .WithColumn("AppEmailText").AsString().Nullable()
                .WithColumn("AppEmailHtml").AsString().Nullable()
                .WithColumn("AppSMTPSecure").AsBoolean().Nullable()
                .WithColumn("AppBlacklist").AsString().Nullable();

        }

        public override void Down()
        {
            Delete.Table("OrderNotification");
            Delete.Table("AppConfiguration");
        }

    }
}
