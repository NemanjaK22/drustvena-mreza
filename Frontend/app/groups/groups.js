const API_BASE_URL = "http://localhost:17948/api/groups";

function initializeGroups() {
  let addBtn = document.querySelector("#addGroupBtn");
  addBtn.addEventListener("click", function () {
    window.location.href = "../groupsForm/groupsForm.html";
  });

  getAllGroups();
}

function getAllGroups() {
  fetch(API_BASE_URL)
    .then((response) => {
      if (!response.ok) {
        throw new Error("Request failed. Status: " + response.status);
      }
      return response.json();
    })
    .then((groups) => renderGroups(groups))
    .catch((error) => {
      console.error("Error", error.message);
      let table = document.querySelector("table");
      if (table) table.style.display = "none";
      alert("An error occurred while loading groups.");
    });
}

function renderGroups(data) {
  let tableBody = document.querySelector("#groups-tbody");
  tableBody.innerHTML = "";
  let tableHeader = document.querySelector("table thead");
  let noDataMessage = document.querySelector("#no-data-message");

  if (data.length === 0) {
    tableHeader.classList.add("hidden");
    noDataMessage.classList.remove("hidden");
  } else {
    tableHeader.classList.remove("hidden");
    noDataMessage.classList.add("hidden");

    data.forEach((group) => {
      let newRow = document.createElement("tr");

      // 1. Naziv grupe
      let cellName = document.createElement("td");
      cellName.textContent = group.name;
      cellName.style.fontWeight = "bold";
      newRow.appendChild(cellName);

      // 2. Datum kreiranja grupe
      let cellDate = document.createElement("td");
      let date = new Date(group.createdDate);
      cellDate.textContent = new Date(group["createdDate"])
        .toISOString()
        .split("T")[0];
      newRow.appendChild(cellDate);

      // 3. Dugme za članove (Ovde ti je deo koda za tvoj A2)
      let cellMembers = document.createElement("td");
      let membersBtn = document.createElement("button");
      membersBtn.textContent = "View Members";
      membersBtn.className = "btn-add";

      membersBtn.addEventListener("click", function () {
        window.location.href =
          "../members/members.html?id=" +
          group["id"] +
          "&name=" +
          group["name"];
      });

      cellMembers.appendChild(membersBtn);
      newRow.appendChild(cellMembers);

      // 4. Dugme za brisanje
      let cellDelete = document.createElement("td");
      let deleteButton = document.createElement("button");
      deleteButton.textContent = "Delete";
      deleteButton.className = "btn-delete";

      deleteButton.addEventListener("click", function () {
        if (confirm("Da li želite da obrišete grupu: " + group.name + "?")) {
          fetch(API_BASE_URL + "/" + group.id, {
            method: "DELETE",
          }).then((response) => {
            if (response.ok) getAllGroups();
          });
        }
      });

      cellDelete.appendChild(deleteButton);
      newRow.appendChild(cellDelete);

      tableBody.appendChild(newRow);
    });
  }
}

document.addEventListener("DOMContentLoaded", initializeGroups);
