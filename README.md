# Blackbird.io Kit

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Kit is an email marketing platform for managing subscribers, broadcasts, and audience communications. The Blackbird Kit app enables you to automate subscriber management, create and update email broadcasts, and build multilingual email workflows using Blackbird and Blacklake.

## Before setting up

Before connecting Kit to Blackbird, make sure that:

- You have an active Kit account.
- You have access to create and manage API keys in Kit.
- Your Kit API key has the permissions needed for the operations you intend to automate.

The app uses the Kit API v4 and authenticates using an API key.

## Connecting

1. In Kit, generate or retrieve your API key.
2. In Blackbird, open the Kit app and select **Add connection**.
3. Enter a recognizable connection name.
4. Enter your Kit API key.
5. Select **Connect** and confirm that the connection is established.

Your API key is treated as a sensitive connection credential.

## Actions

### Broadcasts

- **Search broadcasts** finds broadcasts by status and send date.
- **Get broadcast** returns broadcast details by ID.
- **Create broadcast** creates a new draft broadcast from an HTML file.
- **Download broadcast** downloads broadcast content as interoperable HTML. The subject and preview text are included as translatable fields.
- **Upload broadcast** uploads a translated HTML or XLIFF file. When **Localized broadcast ID** is empty, a new draft is created. Otherwise the specified localized draft is updated.
- **Delete broadcast** deletes a broadcast.
- **Update broadcast** updates an existing broadcast from an HTML file.

### Subscribers

- **Search subscribers** finds subscribers by status, email address and dates.
- **Get subscriber** returns subscriber details by ID.
- **Get subscriber tags** returns all tags of a subscriber.
- **Get subscriber stats** returns email engagement statistics of a subscriber.
- **Create subscriber** creates a subscriber or updates an existing subscriber with the same email address.
- **Add tag to subscriber** applies a tag to a subscriber.
- **Delete tag from subscriber** removes a tag from a subscriber.
- **Update subscriber** updates the email address or first name of a subscriber.
- **Update subscriber custom field** updates a custom field value of a subscriber.
- **Unsubscribe subscriber** unsubscribes a subscriber from all emails.

## Multilingual broadcast workflows

A typical workflow is:

1. **Download broadcast** to get the source broadcast as interoperable HTML.
2. Translate the file with a translation provider.
3. **Upload broadcast** without a localized broadcast ID to create a localized draft in Kit.
4. Optionally send the returned file to Blacklake for multilingual content tracking.
5. **Upload broadcast** with the localized broadcast ID for subsequent revisions.

Uploads never overwrite the source broadcast: the localized broadcast ID must differ from the source broadcast ID, and only drafts can be updated. For XLIFF files, the source and target references are validated as well. The returned file contains the Kit target broadcast reference for subsequent Blackbird and Blacklake steps.

**Important:** Uploading without a localized broadcast ID always creates a new Kit draft. Use the existing localized broadcast ID for subsequent revisions to avoid duplicate drafts.

## Events

### Broadcasts

- **On broadcast published** triggers when a broadcast is published to the web.

### Subscribers

- **On subscriber created** triggers when a new subscriber is created.
- **On subscriber unsubscribed** triggers when a subscriber unsubscribes.

## Additional notes

- Localized broadcasts remain drafts until published in Kit.
- The source broadcast and localized broadcasts have different Kit IDs.
- API availability, permissions, and rate limits are subject to Kit's platform restrictions.

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
