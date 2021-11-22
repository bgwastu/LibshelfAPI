# Libshelf API ![GitHub](https://img.shields.io/github/license/atticdev/LibshelfAPI) ![deploy workflow](https://github.com/atticdev/LibshelfAPI/actions/workflows/deploy-heroku.yml/badge.svg)
Libshelf is book cataloging app that helps you to manage your personal book collection or a home library. This project is a REST API for [Libshelf app](https://github.com/atticdev/libshelf_app). See Swagger live API documentation at https://libshelf.herokuapp.com/.

**Features:**
- Manage books, this include create, read, update and delete.
- Search books by title and ISBN.
- Add progress to the book _(want to read, reading, read)_.
- Login and register users.
- Cataloging the book with book shelves.

Basically just like Goodreads, but it's more minimal and more geared towards personal use.

![swagger-demo](/resources/swagger-demo.jpg)

# Technical
## How to run the project:
- Clone or download this repository
- Copy `appsettings.json` to `appsettings.Development.json`
- Change the secret in `appsettings.Development.json`
- Build the solution using command line with dotnet build
- Go to LibshelfAPI directory and run project using command line with `dotnet run`

## What this project covers:
- JWT implementation
- Authentication and Authorization
- Database many-to-many and one-to-many relationship
- Convert image into .webp
- CRUD operations
- Searching

## Entity relationship diagram (ERD)
![erd](/resources/erd.jpg)
