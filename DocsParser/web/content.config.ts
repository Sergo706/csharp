import { defineContentConfig, defineCollection } from '@nuxt/content'
import z from 'zod';
import { NavigationCollectionSchema } from './shared/types/Navigation'
const buttonVariantEnum = z.enum(['solid', 'outline', 'subtle', 'soft', 'ghost', 'link'])
const cardVariantEnum = z.enum(['solid', 'outline', 'subtle', 'soft', 'ghost', 'naked'])
const colorEnum = z.enum(['primary', 'secondary', 'neutral', 'error', 'warning', 'success', 'info'])
const sizeEnum = z.enum(['xs', 'sm', 'md', 'lg', 'xl'])
const orientationEnum = z.enum(['vertical', 'horizontal'])

const createBaseSchema = () => z.object({
  title: z.string().nonempty(),
  description: z.string().nonempty()
})

const createFeatureItemSchema = () => createBaseSchema().extend({
  icon: z.string().nonempty(),
  to: z.string().optional(),
  target: z.string().optional()
})

const createLinkSchema = () => z.object({
  label: z.string().nonempty(),
  to: z.string().nonempty(),
  icon: z.string().optional(),
  size: sizeEnum.optional(),
  trailing: z.boolean().optional(),
  target: z.string().optional(),
  color: colorEnum.optional(),
  variant: buttonVariantEnum.optional()
})

const createImageSchema = () => z.object({
  src: z.string().nonempty(),
  alt: z.string().optional(),
  title: z.string().optional(),
  height: z.union([z.string(), z.number()]).optional(),
  loading: z.enum(['lazy', 'eager']).optional(),
  srcset: z.string().optional()
})

export default defineContentConfig({
  collections: {

    navigationMenu: defineCollection({
      type: 'data',
      source: 'navigation.json',
      schema: NavigationCollectionSchema
    }),

    index: defineCollection({
      source: 'index.yml',
      type: 'page',
      schema: z.object({
        hero: z.object({
          badge: z.string().optional(),
          image: createImageSchema().optional(),
          links: z.array(createLinkSchema())
        }),
        sections: z.array(
          createBaseSchema().extend({
            icon: z.string().optional(),
            image: createImageSchema().optional(),
            orientation: orientationEnum.optional(),
            reverse: z.boolean().optional(),
            features: z.array(createFeatureItemSchema()).optional(),
            links: z.array(createLinkSchema()).optional()
          })
        ),
        features: createBaseSchema().extend({
          items: z.array(createFeatureItemSchema().extend({
            class: z.string().optional(),
            orientation: orientationEnum.optional(),
            variant: cardVariantEnum.optional()
          }))
        }),
        cta: createBaseSchema().extend({
          links: z.array(createLinkSchema())
        })
      })
    }),

    tools: defineCollection({
      type: 'page',
      source: {
          include: 'tools/**/*.md',
      },
      title: z.string(),
      description: z.string(),
      icon: z.string().optional(),
      image: z.string().optional(),
    }),

    blog_landing: defineCollection({
      type: 'page',
      source: 'blog/index.md',
    }),

    blog: defineCollection({
      type: 'page',
      source: {
        include: 'blog/posts/**',
        prefix: '/blog'
      },
      schema: z.object({
        icon: z.string().optional(),
        tags: z.array(z.string()),
        image: z.string(),
        author: z.string(),
        authorImg: z.string(),
        authorGithub: z.string(),
        authorGithubUserName: z.string(),
        featured: z.boolean(),
        date: z.date(),
        readingTime: z.string(),
        rawbody: z.string().optional()
      })
    }),
  },
})
