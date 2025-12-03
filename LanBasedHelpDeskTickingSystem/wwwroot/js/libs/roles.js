'use strict'

function getRoleById(roleId) {
    switch (roleId) {
        case 0:
            return "Admin";
        case 1:
            return "Technician";
        case 2:
            return "User";
    }
}