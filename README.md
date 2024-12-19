# SAAB Maritime

<img src="SAAB%20MARITIME/Resources/SAABMARITIME.png" alt="SAAB Maritime Logo" width="300" height="300">

Welcome to the SAAB Maritime project! This project is designed to update and track ship positions using AIS (Automatic Identification System) data. The application is structured using the MVC (Model-View-Controller) pattern, although the view component is not included. The program runs as a simple C# console application in Visual Studio.

## Project Structure

```
SAAB MARITIME
├── Controller
│   ├── ShipController.cs
│   └── Updater.cs
├── Model
│   ├── Database
│   │   ├── AISDataImporter.cs
│   │   ├── DatabaseInitializer.cs
│   │   ├── SAMPLE_1hour-vessel-movements-report.csv
│   │   └── aisdk-2024-04-12.csv
│   ├── DatabaseHandler.cs
│   ├── DatabaseQuery.cs
│   ├── Position.cs
│   ├── Ship.cs
│   ├── Vessel.cs
│   └── VesselTrackingCalculator.cs
├── Resources
│   └── SAABMARITIME.png
├── Program.cs
├── SAAB MARITIME.csproj
├── SAAB MARITIME.sln
└── .gitignore
```

## Getting Started

### Prerequisites

- Visual Studio (2019 or later)
- .NET SDK (version compatible with the project)

### Cloning the Repository

1. Open your terminal or command prompt.
2. Navigate to the directory where you want to clone the repository.
3. Run the following command:

   ```bash
   git clone https://github.com/yourusername/SAAB_Maritime.git
   ```

4. Navigate into the cloned directory:

   ```bash
   cd SAAB_Maritime
   ```

### Setting Up the Database

The database is not included in this repository. You can download the required database file from the following link:

[Download AIS Database](https://drive.google.com/file/d/1-REeLFb82DEv65eytNjim8Bg_WLMhULi/view?usp=drive_link)

1. After downloading, place the database file in the `Resources` folder of the project.
2. Update the `databasePath` variable in `Program.cs` to point to the location of the downloaded database file.

### Running the Application

1. Open the solution file `SAAB MARITIME.sln` in Visual Studio.
2. Build the solution to restore any dependencies.
3. Run the application by pressing `F5` or clicking on the "Start" button in Visual Studio.

### Example Usage

In `Program.cs`, you will find an example of how to use the program. The application initializes an `Updater` instance and retrieves ship data based on the specified parameters. It continuously updates the position of the ship until it reaches its destination.

## Contributing

If you would like to contribute to this project, please fork the repository and submit a pull request with your changes.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- This project utilizes AIS data for ship tracking and position updates.
- Special thanks to the contributors and the community for their support.

Feel free to reach out if you have any questions or need further assistance!
