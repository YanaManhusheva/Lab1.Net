using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab1.LINQ.GeneralData;
using Models;

namespace Lab1.LINQ
{
    public class Queries
    {
        readonly Data _data;
        public Queries(Data data)
        {
            _data = data;
        }
        /// <summary>
        /// 1
        /// Joining owners of the project with their project and then grouping by projects Name
        /// </summary>
        public IEnumerable<QueriesPrinter> JoinOwnerProject()
        {
            var joinOP = _data.Projects.GroupJoin(_data.Owners,
                p => p.Code,
                o => o.Project.Code,
                (p, owners) => new { Name = p.Name, Owners = owners }).OrderBy(p => p.Name)
                .Select(p => new QueriesPrinter
                {
                    FieldOne = p.Name,
                    FieldTwo = p.Owners.Select(o => o.Name + " " + o.Surname).Aggregate((s1, s2) => s1 +"\n"+ s2)
                });

            return joinOP;


        }
        /// <summary>
        /// 2
        /// Finding Employyes who work on both projects
        /// </summary>
        public IEnumerable<Employee> IntersectProjectsEmployees()
        {
            var interPE = _data.Projects.ElementAt(0).Employees.Intersect(_data.Projects.ElementAt(1).Employees);
            return interPE;
        }
        
        /// <summary>
        /// 3
        /// Finding the duration of finished projects and sorting them by EndDate
        /// </summary>
        public IEnumerable<QueriesPrinter> FilterDateProjects()
        {
            var filterDP = from projects in _data.Projects
                           where projects.EndDate <= DateTime.Now
                           orderby projects.EndDate
                           select new QueriesPrinter
                           {
                               FieldOne = projects.Name,
                               FieldTwo = ((projects.EndDate - projects.StartDate).TotalDays).ToString()
                           };

            return filterDP;
            
        }
        /// <summary>
        /// 4
        /// Finding the most popular position among Projects
        /// </summary>
        public QueriesPrinter MostPopularPosition()
        {
            var mostPopPosition = _data.Projects.SelectMany(p => p.Employees).
                GroupBy(p => p.Position).
                Select(p => new { Position = p.Key, PeopleCount = p.Count() });

            var maxVal = mostPopPosition.Max(p => p.PeopleCount);
            var popularPos = mostPopPosition.First(p => p.PeopleCount == maxVal);

            return new QueriesPrinter
            {
                FieldOne = popularPos.Position.ToString(),
                FieldTwo = popularPos.PeopleCount.ToString()
            };
            
            
        }
        /// <summary>
        /// 5
        /// Finding the amount of workers on each project and grouping by the number of employees
        /// </summary>
        public IEnumerable<QueriesPrinter> NumberEmployees()
        {
            var numberEmployees = _data.Projects.Select(p => new { EmployeesNumber = p.Employees.Count, ProjectName = p.Name }).
                GroupBy(p => p.EmployeesNumber).
                Select(u =>
                new QueriesPrinter
                {
                    FieldOne = u.Key.ToString(), 
                    FieldTwo = u.Select(p => p.ProjectName).Aggregate((s1, s2) => s1 + " , " + s2) 
                });
            return numberEmployees;

            
        }
        /// <summary>
        /// 6
        /// Grouping by Employees Position and finding their percentage among all Positions
        /// </summary>
        public IEnumerable<QueriesPrinter> PercentageOfPositions()
        {
            var allEmployyesCount = _data.Employees.Count();


            var percentagePos = from emp in _data.Employees
                                group emp by emp.Position into empPositions
                                select new QueriesPrinter
                                {
                                    FieldOne = empPositions.Key.ToString(),
                                    FieldTwo = ((double)empPositions.Count() / allEmployyesCount * 100).ToString()
                                };
            return percentagePos;
           
        }
        /// <summary>
        /// 7
        /// Finding owners Income depending on the project cost
        /// </summary>
        public IEnumerable<QueriesPrinter> EmployerMoney()
        {

            var empMoney = from result in (from owners in _data.Owners
                                           group owners by owners.Project into ownersProj
                                           select new
                                           {
                                               ProjectName = ownersProj.Key.Name,
                                               FullName = from ownPj in ownersProj
                                                          select new string(ownPj.Name + " " + ownPj.Surname),
                                               Salary = (ownersProj.Key.ProjectCost / ownersProj.Key.Owners.Count) / 40 * 100
                                           })
                           orderby result.Salary
                           select new QueriesPrinter
                           {
                               FieldOne = result.ProjectName,
                               FieldTwo = result.FullName.Count() < 2 
                               ? result.FullName.First() + $" - Salary: {result.Salary}"
                               : result.FullName.Aggregate((s1, s2) => s1 + $" - Salary: {result.Salary} \n" + s2 + $" - Salary: {result.Salary}")
                           };

            return empMoney;
            
        }
        /// <summary>
        /// 8
        /// Rating Employees depending on their salary
        /// </summary>
        public IOrderedEnumerable<QueriesPrinter> EmployeeRating()
        {
            var emplRating = from res in (from employees in _data.Employees
                                          select new QueriesPrinter
                                          {
                                              FieldOne = employees.Name + " " + employees.Surname,
                                              FieldTwo = (from salaryProjects in employees.Projects
                                                        select Decimal.Round(salaryProjects.ProjectCost 
                                                        / salaryProjects.Employees.Count / 60 * 100)).Sum().ToString()
                                          })
                             orderby res.FieldTwo descending
                             select res;
            return emplRating;
            
            

        }
        /// <summary>
        /// 9
        /// The most popular month of Starting projects
        /// </summary>
        public string MostPopularStartingMonth()
        {
            var startMonth = _data.Projects.Select(p => new
            {
                Name = p.Name,
                Owners = p.Owners.Select(o => o.Name + " " + o.Surname).Aggregate((s1, s2) => s1 + "," + s2),
                Month = p.StartDate.Month
            }).GroupBy(d => d.Month).
            Select(p => new
            {
                Month = p.Key,
                Count = p.Count(),
                Name = p.Select(n => n.Name).Aggregate((s1, s2) => s1 + "," + s2),
                Owners = p.Select(n => n.Owners).Aggregate((s1, s2) => s1 + "," + s2)
            }).
            OrderByDescending(p => p.Count)
            .First();

            Console.WriteLine("\nQueue 9");

           return ($"\nMonth - {startMonth.Month}, " +
                $"\nAmount of projects - {startMonth.Count}, " +
                $"\nName - {startMonth.Name}, " +
                $"\nOwners - {startMonth.Owners}");
           
        }
        /// <summary>
        /// 10
        /// Finding employees who don`t work on First and Last projects
        /// </summary>
        public IEnumerable<QueriesPrinter> DontWorkEmployee()
        {
            var dontWorkEmpl = _data.Employees.
                Except(_data.Projects.First().Employees.
                Union(_data.Projects.Last().Employees)).Select(p => new QueriesPrinter{ FieldOne = p.Name, FieldTwo = p.Surname });

            return dontWorkEmpl;
        }
        /// <summary>
        /// 11
        /// Finding Avarage project cost per month
        /// </summary>
        public IOrderedEnumerable<QueriesPrinter> AverageProjectCost()
        {
            var averageProjCost = from result in (from projects in _data.Projects
                                                  select new QueriesPrinter
                                                  {
                                                      FieldOne = projects.Name,
                                                      FieldTwo = Decimal.Round(projects.ProjectCost / (decimal)
                                                      (projects.EndDate > DateTime.Now ?
                                                      (DateTime.Now.Subtract(projects.StartDate).Days / (365.25 / 12)) :
                                                      ((projects.EndDate).Subtract(projects.StartDate).Days / (365.25 / 12)))).ToString()
                                                  })
                                  orderby result.FieldTwo descending
                                  select result;
            return averageProjCost;
        }

        /// <summary>
        /// 12
        /// Finding General salary of all workers on a certain Position
        /// </summary>
        public IEnumerable<QueriesPrinter> PositionCost()
        {
            var positionCost = _data.Employees.Select(e => new
            {
                Position = e.Position,
                GeneralSalary = Decimal.Round(e.Projects.Sum(p => p.ProjectCost / p.Employees.Count / 60 * 100))
            }).GroupBy(p => p.Position).Select(u => new QueriesPrinter
            {
                FieldOne = u.Key.ToString(), 
                FieldTwo = u.Sum(p => p.GeneralSalary).ToString() 
            });
            return positionCost;

        }

        /// <summary>
        /// 13
        /// Find and union all stakeholders to a certain project
        /// </summary>
        public IEnumerable<QueriesPrinter> AllStakeholders()
        {
            var allStakeholders = _data.Projects.Select(e => new QueriesPrinter {
                FieldOne = e.Name,
                FieldTwo = e.Employees.OfType<Person>().Union(e.Owners).Select(e => (e.Name + " " + e.Surname)).Aggregate((s1,s2)=> s1+ "\n"+s2) }
           );
            return allStakeholders;
        }
        /// <summary>
        /// 14
        /// Finding the percantage each project has depending on a general cost of all projects
        /// </summary>
        public IEnumerable<QueriesPrinter> PercentageCostProjects()
        {
            var percentageCostProjects = from projects in _data.Projects
                                         select new QueriesPrinter
                                         {
                                             FieldOne = projects.Name,
                                             FieldTwo = Decimal.Round(projects.ProjectCost * 100 /
                                             (from prCost in _data.Projects
                                             select prCost.ProjectCost).Sum(),2).ToString()
                                         };

            return percentageCostProjects;
            
        }

        /// <summary>
        /// 15
        /// Finding best proposed project for an employee depending on his possible Salary
        /// </summary>
        public IEnumerable<QueriesPrinter> BestProposal()
        {
            var bestProposal = _data.Employees.Select(e => new QueriesPrinter
            {
                FieldOne = e.Name + " " + e.Surname,
                FieldTwo = _data.Projects.Except(e.Projects).Any() 
                ? 
                _data.Projects.Except(e.Projects).
                Select(p => new 
                {
                    ProjectName = p.Name, 
                    NewSalary = Decimal.Round(p.ProjectCost / (p.Employees.Count + 1) / 60 * 100) 
                }).
                OrderByDescending(p => p.NewSalary)
                .Select(e => $"Project : {e.ProjectName} \tSalary : {e.NewSalary}").Aggregate((s1, s2) => s1 + "\n" + s2)
                : "Already on all projects"
                
            }) ;

            return bestProposal;
        }
    }

    public class QueriesPrinter 
    {
        public string FieldOne { get; init; }
        public string FieldTwo { get; init; }
    }




}
