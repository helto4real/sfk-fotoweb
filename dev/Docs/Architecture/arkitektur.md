# Arkitektur
Detta dokument beskriver den övergripande arkitekturen i projektet.

# Domänanalys

### Medlemsadministration
- Administratör kan tilldela roller till medlemmar
- Administratör kan förregistrera en e-post
- Administratör kan ta bort en medlem som slutat

### Medlemmar

Användningsfall:
- Medlemmar registrerar sitt konto med hjälp av en länk som innehåller en token som ger de rättighet att registrera ett konto om administratören har förregistrerat e-post adressen. 
- Medlemmar kan ändra sina uppgifter i systemet
- Medlemmar kan avregistrera sig i systemet 

### Tävlingar

Användningsfall:
- Tävlingsadministratör kan skapa nya tävlingar av en viss typ
- Medlem medlem kan ladda upp bilder till en specifik tävling genom att 
- Medlem kan ladda upp digital kopia av påsiktsbild genom att logga in
- Medlem kan lista de bilder som deltar i tävlingar
- Domare titta på, bedöma, skriva kommentarer och ranka bilder i en tävling
- Domare kan sätta status att en tävling är klar
- Tävlingsadministratör kan göra resultat och kommentarer tillgängliga för medlemmar efter redovisning

### Fotouppladdningar

#### Månadens bild
- Medlemmar eller användare med kod kan ladda upp bilder för visning med de regelverk som gäller för typen av visning
- Visa uppladdade bilder 

#### ST-bilder
- Medlemmar laddar upp bilder till för ST-inlämning med uppgifter som ST vill ha
- ST-administratör godkänner bilder
- ST-administratör paketerar bilder och skickar till ST

### Bilder
- Bilder kan lagras genom fördefinierade regler om storlek och typ
- Bilder kan visas som ikon eller i fullskärm

### Infrastruktur
- Notifieringar via e-post
- Administratör hanterar accesstokens
- Administratör hanterar accesskoder