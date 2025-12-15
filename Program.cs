using System.Transactions;
using System.Xml.Serialization;

namespace Day15EmployeeManagementApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EmployeeService empService = new EmployeeService();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1. Add Employee");
                Console.WriteLine("2. View Employees");
                Console.WriteLine("3. Exit");
                int choice;

                try
                {
                    choice = int.Parse(Console.ReadLine());
                }
                catch (FormatException)
                {
                    choice = 0;
                }

                if (choice == 1)
                {
                    try
                    {
                        empService.AddEmployee();
                    }
                    catch (ValidException ex)
                    {
                        Console.WriteLine("Error storing data. " + ex.Message);
                        Console.WriteLine("Retry.....");
                    }
                }
                else if (choice == 2)
                {
                    try
                    {
                        var allEmps = empService.GetEmployees();

                        if (allEmps.Count > 0)
                        {
                            Console.WriteLine("All Employees:");
                        }

                        foreach (var emp in allEmps)
                        {
                            Console.WriteLine($"{emp.Id} - {emp.Name} - {emp.Salary}");
                        }
                    }
                    catch (ValidException ex)
                    {
                        Console.WriteLine("Error Getting Data. " + ex.Message);
                        Console.WriteLine("Retry.....");
                    }
                }
                else if (choice == 3)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please select again..");
                }
            }
        }
    }

    // MODEL
    class Employee
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public double Salary { get; private set; }

        public Employee(int id, string name, double salary)
        {
            Id = id;
            Name = name;
            Salary = salary;
        }

    }

    class EmployeeService
    {
        EmployeeFileStore store = new EmployeeFileStore();
        private List<Employee> employees = new List<Employee>();

        private void GetEmployeeDetails()
        {
            int id; string name; double salary;
            try
            {
                Console.Write("Id: ");
                id = int.Parse(Console.ReadLine());

                Console.Write("Name: ");
                name = Console.ReadLine();

                Console.Write("Salary: ");
                salary = double.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                throw new ValidException("Please provide correct data");
            }
            catch (IOException)
            {
                throw new ValidException("I/O Error");
            }

            if (salary < 0)
            {
                throw new ValidException("Salary can not be negative");
            }

            SaveEmployee(new Employee(id, name, salary));
        }

        private void SaveEmployee(Employee employee)
        {
            store.SaveData(employee);
        }

        public void AddEmployee()
        {
            try
            {
                GetEmployeeDetails();
            }
            catch (ValidException ex)
            {
                throw new ValidException(ex.Message);
            }
        }

        public List<Employee> GetEmployees()
        {
            employees.Clear();
            try
            {
                employees = store.GetData();
            }
            catch (ValidException ex)
            {
                Console.WriteLine(ex.Message);
                employees.Clear();
            }
            return employees;
        }
    }

    class EmployeeFileStore
    {
        private string filepath = "employee.txt";

        public void SaveData(Employee emp)
        {
            using (StreamWriter sw = new StreamWriter(filepath, true))
            {
                sw.WriteLine($"{emp.Id},{emp.Name},{emp.Salary}");
            }
        }

        public List<Employee> GetData()
        {
            List<Employee> employees = new List<Employee>();
            int id; string name; double salary;

            if (!File.Exists(filepath))
            {
                throw new ValidException("Filepath Invalid. Error: Unable to get data");
            }

            using (StreamReader sw = new StreamReader(filepath))
            {
                string? line;

                while ((line = sw.ReadLine()) != null)
                {
                    var parts = line.Split(",");
                    try
                    {
                        id = int.Parse(parts[0]);
                        name = parts[1];
                        salary = double.Parse(parts[2]);
                    }
                    catch (FormatException)
                    {
                        throw new ValidException("Invalid data type present in file. Error: Unable to get data.");
                    }
                    employees.Add(new Employee(id, name, salary));
                }
            }

            return employees;
        }
    }

    class ValidException : Exception
    {
        public ValidException(string message) : base(message) { }
    }
}
