using MuniConnect.Models;

namespace MuniConnect.Data
{
    public class IssueRepository
    {
        private readonly IssueLinkedList<Issue> _issues = new IssueLinkedList<Issue>();
        private int _idCounter = 1;

        public IEnumerable<Issue> GetAll()
        {
            return _issues;
        }

        public void Add(Issue issue)
        {
            issue.Id = _idCounter++;
            issue.DateReported = DateTime.Now;
            _issues.Add(issue);
        }

        public Issue? GetById(int id)
        {
            foreach (var issue in _issues)
            {
                if (issue.Id == id)
                    return issue;
            }
            return null;
        }
    }
}
