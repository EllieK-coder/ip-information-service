# ip-finder
# How to Work on an Issue

A simple step-by-step guide for our GitHub workflow. Follow these in order every time you pick up an issue.

---

## 1. Start from the latest main

Make sure you're on `main` and have the newest code before branching:

```bash
git checkout main
git pull origin main
```

## 2. Create a branch for your issue

Never work directly on `main`. Make a new branch named after what you're doing:

```bash
git checkout -b feature/short-description
```

Example: `git checkout -b feature/ip2c-gateway` or `git checkout -b fix/cache-key-bug`.

## 3. Do your work

Write your code. Save your files. Test that it builds and runs.

## 4. Stage and commit your changes

Pick up your changes and lock them in with a clear message:

```bash
git add .
git commit -m "Add IP2C gateway client with status parsing"
```

Write the message so a teammate understands what you did.

## 5. Push your branch to GitHub

The **first** time you push this branch:

```bash
git push -u origin feature/short-description
```

(Use the same branch name from step 2.) After that first push, any later pushes on the same branch are just `git push`.

## 6. Open a Pull Request

- Go to the repository page on GitHub.
- You'll see a **"Compare & pull request"** button near the top — click it.
- Fill in the PR template (what you did, link the issue with `Closes #123`).
- Click **Create pull request**.

## 7. Wait for review

- Your PR needs **2 approvals** before it can merge.
- If a reviewer asks for changes: make them locally, then `git add` → `git commit` → `git push` again. The PR updates automatically — no need to open a new one.
- Once approved, merge it. Done!

---

## Quick reference

```bash
git checkout main
git pull origin main
git checkout -b feature/my-thing
# ... do your work ...
git add .
git commit -m "Clear message here"
git push -u origin feature/my-thing
# then open the PR on GitHub
```

## Golden rules

- **Never work directly on `main`** — always a branch.
- **Pull main before branching** so you start from the latest.
- **Small PRs** are easier to review than huge ones.
- **Link the issue** in your PR (`Closes #123`) so it closes automatically on merge.
