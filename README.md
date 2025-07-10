# APRS Thursday Message Automation

[\#APRSThursday](https://aprsph.net/aprsthursday) is a worldwide APRS messaging net held every Thursday and managed by Michael KC8OWL. The purpose is to increase APRS message traffic globally.
This project is an Azure Function designed to send a predefined APRS message every Thursday. It can also be triggered manually via an HTTP request. It can also be run using Docker.

## Disclaimer

You should get callsign and passcode from your local amateur radio authority. **Do not use this project to spam the APRS network.**
This project is **not affiliated with or endorsed by** the official #APRSThursday net or its organizers.  
It is an independent tool created to help automate participation in the #APRSThursday event.

## How to Run

### From Docker (No Azure Functions Core Tools Needed)

To run this project using Docker, you can build and run the Docker image as follows:
Change the environment variables to match your configuration.

```bash
docker build -t aprs-thursday-message .
docker run -p 7071:80 -e FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -e APRS_SERVER_URL=https://ametx.com:8888 -e CALLSIGN=YOUR_CALLSIGN -e PASSCODE=YOUR_PASSCODE -e VERSION="AprsThursdayAutomation 1.0" -e APRS_THURSDAY_MESSAGE_TO=ANSRVR -e APRS_THURSDAY_MESSAGE_TEXT="CQ HOTG TURKIYE 73!" aprs-thursday-message
```

### From Azure or Local Development

1. Clone the repository.
2. Create a `local.settings.json` file from `local.settings.example.json` and update the values.
3. Run the project using the Azure Functions Core Tools or from your IDE (like Visual Studio or VS Code).

## Configuration

To run this function, you need to configure the following settings. You can use the `local.settings.example.json` file as a template by renaming it to `local.settings.json` and filling in the values.

- `APRS_SERVER_URL`: The URL of the APRS-IS server. Defaults to `https://ametx.com:8888` in the example.
- `CALLSIGN`: Your amateur radio callsign.
- `PASSCODE`: Your APRS-IS passcode for your callsign. You can generate a passcode at [MagicBug Passcode Generator](https://apps.magicbug.co.uk/passcode/).
- `VERSION`: The version of your client. Defaults to `AprsThursdayAutomation 1.0` in the example.
- `APRS_THURSDAY_MESSAGE_TO`: The destination callsign for the APRS message.
- `APRS_THURSDAY_MESSAGE_TEXT`: The content of the message to be sent.

For \#APRSThursday, you should use these values. You can change TURKIYE to your own country.

- APRS_THURSDAY_MESSAGE_TO: "ANSRVR"
- APRS_THURSDAY_MESSAGE_TEXT: "CQ HOTG TURKIYE 73!"

### `local.settings.example.json`

```json
{
    "IsEncrypted": false,
    "Values": {
      "AzureWebJobsStorage": "UseDevelopmentStorage=true",
      "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
      "APRS_SERVER_URL": "https://ametx.com:8888",
      "CALLSIGN": "YOUR_CALLSIGN",
      "PASSCODE": "YOUR_PASSCODE",
      "VERSION": "AprsThursdayAutomation 1.0",
      "APRS_THURSDAY_MESSAGE_TO": "DEST_CALLSIGN",
      "APRS_THURSDAY_MESSAGE_TEXT": "Your message here!"
    }
}
```

## Triggers

### Timer Trigger

- **Function:** `AprsThursdayMessageRun`
- **Schedule:** Every Thursday at 12:00 PM (`0 12 * * 4`). If you want to change the schedule, modify the cron expression in the `TimerTrigger` attribute.

### Manual HTTP Trigger

- **Function:** `AprsThursdayMessageTriggerManually`
- **Endpoint:** `POST` to `/api/AprsThursdayMessageTriggerManually`.
- **Authorization:** Function level. You will need to provide the function key.
