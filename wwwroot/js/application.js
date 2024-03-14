class Application {

    // form properties
    age = null;
    language = null;
    budget = null;

    // data location
    dataDiv = null;

    validateForm() {
        // get form values
        this.age = document.getElementById('age').value;
        this.language = document.getElementById('language').value;
        this.budget = document.getElementById('budget').value;
        console.log(("Age: " + this.age));
        console.log(("Language: " + this.language));
        console.log(("Budget: " + this.budget));

        // if age and budget are not populated, show the error
        const errorDiv = document.getElementById('error-text');
        if (!this.age || !this.budget) {
            errorDiv.style.visibility = "visible";
            return false;
        } else {
            errorDiv.style.visibility = "hidden";
            return true;
        }
    }

    processForm() {
        // clear any displayed data
        this.clearData();

        // only do this is we have validated the form
        if (this.validateForm()) {

            // set the api data source
            const url = 'https://localhost:7102/GetGamesForUser?age=' + this.age + "&budget=" + this.budget + "&language=" + this.language;

            // make api call to get the data
            fetch(url)
                .then((response) => response.json())
                .then((json) => {
                    console.log(json);
                    this.makeTable(json);
                }
                );
        }
    }

    clearData() {
        this.dataDiv = document.getElementById('data-content');
        // remove all child elements
        let child = this.dataDiv.lastElementChild;
        while (child) {
            this.dataDiv.removeChild(child);
            child = this.dataDiv.lastElementChild;
        }
    }

    makeTable(data) {
        // create table and tbody elements
        const tbl = document.createElement("table");
        const tblBody = document.createElement("tbody");

        // create a table header row
        let row = document.createElement("tr");
        this.addCell('th', 'Entry', 'header-number', row);
        this.addCell('th', 'Name', 'header-name', row);
        this.addCell('th', 'Description', 'header-description', row);
        this.addCell('th', 'Developer', 'header-developer', row);
        this.addCell('th', 'Age Rating', 'header-age-rating', row);
        this.addCell('th', 'Price', 'header-price', row); // Added a new header for Price
        this.addCell('th', 'Release Date', 'header-release-date', row); // Added a new header for Release Date
        tblBody.appendChild(row);

        // creating all cells
        for (let i = 0; i < data.length; i++) {
            // creates a table row
            row = document.createElement("tr");

            this.addCell('td', '#' + (i + 1), 'row-number', row);
            this.addCell('td', data[i].data.name, 'row-name', row, 'https://store.steampowered.com/app/' + data[i].data.steamAppId);
            this.addCell('td', data[i].data.shortDescription, 'row-id description', row);
            this.addCell('td', data[i].data.developers, 'row-developer', row);
            this.addCell('td', data[i].data.mostCommonAgeRating + '+', 'row-age-rating', row);
            this.addCell('td', data[i].data.priceOverview ? data[i].data.priceOverview.finalFormatted : 'Free', 'row-price', row); // if final formatted price is null, game must be free
            this.addCell('td', data[i].data.releaseDate.date, 'row-release-date', row); // Added a new cell for Release Date

            // add the row to the end of the table body
            tblBody.appendChild(row);
        }

        // put the <tbody> in the <table>
        tbl.appendChild(tblBody);
        // appends <table> into <body>
        this.dataDiv.appendChild(tbl);
    }

    addCell(type, text, className, row, link = null) {
        const cell = document.createElement(type);
        let cellText;
        if (link) {
            cellText = document.createElement('a');
            cellText.href = link;
            cellText.textContent = text;
        } else {
            cellText = document.createTextNode(text);
        }
        cell.appendChild(cellText);
        cell.className = className;
        if (className === 'row-release-date') {
            cell.classList.add('cell-no-wrap');
        }
        row.appendChild(cell);

        // prevent user from using scientific notation
        document.getElementById('age').addEventListener('input', function (e) {
            e.target.value = e.target.value.replace(/[^0-9]/g, '').replace(/(\..*)\./g, '$1').slice(0, 2);
        });
        document.getElementById('budget').addEventListener('input', function (e) {
            e.target.value = e.target.value.replace(/[^0-9]/g, '').replace(/(\..*)\./g, '$1');
        });
    }
}

app = new Application();