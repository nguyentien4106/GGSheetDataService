using GoogleSheetsWrapper;

namespace DataWorkerService.Models.Sheet
{
    public class Repository : BaseRepository<Record>
    {
        public Repository() { }

        public Repository(SheetHelper<Record> sheetsHelper, BaseRepositoryConfiguration config)
            : base(sheetsHelper, config) { }
    }
}
