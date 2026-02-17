function initialize() {

    const urlParams = new URLSearchParams(window.location.search)
    const name = urlParams.get('name')

    if (name) {
        let titles = document.querySelectorAll('h2')
        titles.forEach(title => {
            title.textContent = title.textContent.split('group')[0] + '"' + name + '" group';
        })
    }

    const id = urlParams.get('id') 
    if (!id) { 
        return
    }

    getUsersInGroup(id)
}
function getUsersInGroup(id) {
    fetch(`http://localhost:17948/api/groups/${id}/users`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Request failed. Status: ' + response.status)
            }
            return response.json()
        })
        .then(users => {
            renderData('#in-group tbody', users)
            getUsersOutOfGroup(users)
        })
        .catch(error => {
            console.error('Error:', error.message)
            alert('An error occurred while loading the data. Please try again.')
        })
}

function getUsersOutOfGroup(usersInGroup) {
    fetch(`http://localhost:17948/api/users`) 
        .then(response => {
            if (!response.ok) {
                throw new Error('Request failed. Status: ' + response.status)
            }
            return response.json()
        })
        .then(users => {
            let usersOutOfGroup = []
            for (let user of users) {
                let inGroup = false

                for (let userInGroup of usersInGroup) {
                    if (user.id === userInGroup.id) {
                        inGroup = true
                        break
                    }
                }

                if (!inGroup) {
                    usersOutOfGroup.push(user)
                }
            }

            renderData('#out-of-group tbody', usersOutOfGroup)
        })
        .catch(error => {
            console.error('Error:', error.message)
            // Prikaži poruku o grešci
            alert('An error occurred while loading the data. Please try again.')
        })
}

function renderData(tableBodySelector, data) {
    let tableBody = document.querySelector(tableBodySelector)
    tableBody.innerHTML = '' 

    data.forEach(user => {
        let newRow = document.createElement('tr')

        let cell1 = document.createElement('td')
        cell1.textContent = user['username']
        newRow.appendChild(cell1)

        let cell2 = document.createElement('td')
        cell2.textContent = user['firstName']
        newRow.appendChild(cell2)

        let cell3 = document.createElement('td')
        cell3.textContent = user['lastName']
        newRow.appendChild(cell3)

        let cell4 = document.createElement('td')
        cell4.textContent = new Date(user['dateOfBirth']).toISOString().split('T')[0]
        newRow.appendChild(cell4)

        let cell5 = document.createElement('td')
        let button = document.createElement('button')
        let method = 'DELETE';
        if (tableBodySelector.includes('in')) {
            button.textContent = 'Remove'
        } else {
            button.textContent = 'Insert'
            method = 'PUT'
        }
        const urlParams = new URLSearchParams(window.location.search)
        const groupId = urlParams.get('id')
        let userId = user['id'];
        button.addEventListener('click', function () {
            fetch(`http://localhost:17948/api/groups/${groupId}/users/${userId}`, { method: method })
                .then(response => {
                    if (!response.ok) {
                        const error = new Error('Request failed. Status: ' + response.status)
                        error.response = response 
                        throw error  
                    }
                    getUsersInGroup(groupId) 
                })
                .catch(error => {
                    console.error('Error:', error.message)
                    if (error.response && error.response.status === 404) {
                        alert('Group or user does not exist!')
                    } else {
                        alert('An error occurred while deleting the group. Please try again.')
                    }
                })
        })
        cell5.appendChild(button)
        newRow.appendChild(cell5)

        tableBody.appendChild(newRow)
    })
}

document.addEventListener('DOMContentLoaded', initialize)