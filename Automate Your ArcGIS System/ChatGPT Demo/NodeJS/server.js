import http from "http";
import OpenAI from "openai";

const openai = new OpenAI();

const server = http.createServer(async(req, res) => {
  res.statusCode = 200;
  res.setHeader("Content-Type", "text/plain");
  res.setHeader('Access-Control-Allow-Origin', '*');
  res.setHeader('Access-Control-Allow-Methods', 'OPTIONS, GET');

  const completion = await openai.chat.completions.create({
    messages: [{ role: "system", content: "Geef in JSON formaat de naam en een beschrijving, max 50 tekens, van 5 andere bekende cafes inclusief coordinaten die liggen in Nijmegen waarbij de naam is geschreven als Naam, de beschrijving als Beschrijving en de coordinaten in het coordinaten systeem WGS84 als array met de naam Locatie, alles is samengevoegd onder de naam cafes" }],
    model: "gpt-3.5-turbo",
  });

  console.log(completion.choices[0].message.content);
  res.end(completion.choices[0].message.content);
});

const PORT = 3001;
server.listen(PORT, () => {
  console.log(`Server running at http://localhost:${PORT}/`);
});